using EI.SI;
using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Threading;

namespace ChatServer
{
    /// <summary>
    /// Gere todos os clientes ligados ao servidor.
    /// Mantém a lista de ClientHandlers e a chave RSA do servidor.
    /// </summary>
    public class GestorClientes
    {
        private readonly List<ClientHandler> clients    = new List<ClientHandler>();
        private readonly object clientsLock             = new object();
        private int clientCounter                       = 0;
        private readonly Logger logger;
        private readonly GestorUtilizadores gestorUtilizadores;

        // Par de chaves RSA do servidor — gerado uma vez no arranque
        private readonly RSACryptoServiceProvider rsaServidor;
        public string ChavePublicaServidor { get; private set; }

        public GestorClientes(Logger logger)
        {
            this.logger               = logger;
            this.gestorUtilizadores   = new GestorUtilizadores();

            rsaServidor            = new RSACryptoServiceProvider();
            ChavePublicaServidor   = rsaServidor.ToXmlString(false); // só chave pública
            logger.Info("Chave RSA do servidor gerada.");
        }

        /// <summary>
        /// Cria um novo ClientHandler para o cliente TCP recebido e inicia a sua thread.
        /// </summary>
        public ClientHandler AdicionarCliente(TcpClient tcpClient)
        {
            clientCounter++;
            int id = clientCounter;

            Console.WriteLine("[{0}] Novo cliente conectado (ID: {1})", DateTime.Now.ToLongTimeString(), id);
            logger.Info($"Cliente ID:{id} conectado (IP: {tcpClient.Client.RemoteEndPoint})");

            ClientHandler handler = new ClientHandler(
                tcpClient, id, clients, clientsLock, this, logger, gestorUtilizadores);

            lock (clientsLock) { clients.Add(handler); }
            handler.Start();
            return handler;
        }

        /// <summary>
        /// Remove um cliente da lista de clientes ativos.
        /// </summary>
        public void RemoverCliente(ClientHandler handler)
        {
            lock (clientsLock) { clients.Remove(handler); }
        }

        /// <summary>
        /// Envia uma mensagem em texto simples para todos os clientes exceto o remetente.
        /// Cada ClientHandler cifra a mensagem com a sua própria chave AES antes de enviar.
        /// </summary>
        public void BroadcastMessage(ClientHandler remetente, string mensagemPlana)
        {
            List<ClientHandler> snapshot;
            lock (clientsLock) { snapshot = new List<ClientHandler>(clients); }

            foreach (ClientHandler client in snapshot)
            {
                if (client != remetente)
                    client.EnviarMensagemCifrada(mensagemPlana);
            }
        }
    }

    /// <summary>
    /// Representa a ligação de um cliente individual.
    /// Trata o handshake de segurança, autenticação e troca de mensagens cifradas.
    /// </summary>
    public class ClientHandler
    {
        private readonly TcpClient tcpClient;
        private readonly NetworkStream networkStream;
        private readonly int clientID;
        private string username;

        private readonly List<ClientHandler> allClients;
        private readonly object clientsLock;
        private readonly GestorClientes gestor;
        private readonly Logger logger;
        private readonly GestorUtilizadores gestorUtilizadores;
        private readonly object sendLock = new object();

        // Chave pública RSA do cliente — recebida no handshake
        public string ChavePublicaCliente { get; private set; }

        // Chave AES simétrica gerada pelo servidor para esta sessão
        private byte[] chaveAES;
        private byte[] ivAES;

        public ClientHandler(TcpClient client, int id, List<ClientHandler> clients,
            object lockObj, GestorClientes gestor, Logger logger, GestorUtilizadores gestorUtilizadores)
        {
            this.tcpClient          = client;
            this.clientID           = id;
            this.allClients         = clients;
            this.clientsLock        = lockObj;
            this.gestor             = gestor;
            this.logger             = logger;
            this.gestorUtilizadores = gestorUtilizadores;
            this.networkStream      = client.GetStream();
        }

        /// <summary>
        /// Inicia a thread de tratamento deste cliente em background.
        /// </summary>
        public void Start()
        {
            Thread thread = new Thread(Handle);
            thread.IsBackground = true;
            thread.Start();
        }

        /// <summary>
        /// Loop principal de receção de mensagens do cliente.
        /// Encaminha cada tipo de pacote para o método adequado.
        /// </summary>
        private void Handle()
        {
            ProtocolSI protocolSI = new ProtocolSI();

            try
            {
                while (true)
                {
                    int bytesRead = networkStream.Read(protocolSI.Buffer, 0, protocolSI.Buffer.Length);
                    if (bytesRead == 0) break;

                    switch (protocolSI.GetCmdType())
                    {
                        case ProtocolSICmdType.USER_OPTION_1:
                            if (!HandleHandshake(protocolSI)) return;
                            break;

                        case ProtocolSICmdType.DATA:
                            HandleMensagem(protocolSI);
                            break;

                        case ProtocolSICmdType.EOT:
                            HandleDesconexao();
                            return;
                    }
                }
            }
            catch (Exception ex)
            {
                string who = username ?? ("ID " + clientID);
                Console.WriteLine("[Erro] Cliente {0}: {1}", clientID, ex.Message);
                logger.Error($"Cliente {who}: {ex.Message}");
            }
            finally
            {
                gestor.RemoverCliente(this);
                networkStream.Close();
                tcpClient.Close();
                logger.Info($"Cliente ID:{clientID} recursos libertados.");
            }
        }

        /// <summary>
        /// Realiza o handshake completo de segurança com o cliente:
        /// 1. Recebe username (USER_OPTION_1)
        /// 2. Envia chave pública RSA do servidor (DATA)
        /// 3. Recebe chave pública RSA do cliente (DATA)
        /// 4. Gera chave AES, cifra com RSA do cliente, envia (USER_OPTION_2)
        /// 5. Recebe credenciais cifradas com AES (USER_OPTION_3)
        /// 6. Valida autenticação com SHA-512 + salt
        /// 7. Envia ACK (sucesso) ou NACK (falha)
        /// </summary>
        /// <returns>True se autenticação bem sucedida; False caso contrário.</returns>
        private bool HandleHandshake(ProtocolSI protocolSI)
        {
            // PASSO 1: Receber username
            username = protocolSI.GetStringFromData();
            Console.WriteLine("[{0}] Cliente {1} identificado como '{2}'",
                DateTime.Now.ToLongTimeString(), clientID, username);

            // PASSO 2: Enviar chave pública RSA do servidor
            byte[] packetPubKey = protocolSI.Make(ProtocolSICmdType.DATA, gestor.ChavePublicaServidor);
            lock (sendLock) { networkStream.Write(packetPubKey, 0, packetPubKey.Length); }

            // PASSO 3: Receber chave pública RSA do cliente
            networkStream.Read(protocolSI.Buffer, 0, protocolSI.Buffer.Length);
            ChavePublicaCliente = protocolSI.GetStringFromData();
            logger.Info($"Chave pública RSA do cliente '{username}' recebida.");

            // PASSO 4: Gerar chave AES-256 + IV, cifrar com RSA do cliente e enviar
            GestorCriptografia.GerarChaveAES(out chaveAES, out ivAES);

            // Concatenar chave (32 bytes) + IV (16 bytes) = 48 bytes
            byte[] aesData = new byte[chaveAES.Length + ivAES.Length];
            Buffer.BlockCopy(chaveAES, 0, aesData, 0,                chaveAES.Length);
            Buffer.BlockCopy(ivAES,   0, aesData, chaveAES.Length,   ivAES.Length);

            byte[] aesDataCifrado = GestorCriptografia.CifrarComRSA(aesData, ChavePublicaCliente);
            string aesBase64      = Convert.ToBase64String(aesDataCifrado);

            byte[] packetAES = protocolSI.Make(ProtocolSICmdType.USER_OPTION_2, aesBase64);
            lock (sendLock) { networkStream.Write(packetAES, 0, packetAES.Length); }
            logger.Info($"Chave AES gerada e enviada cifrada com RSA para '{username}'.");

            // PASSO 5: Receber credenciais cifradas com AES
            // Formato: "LOGIN|username|password" ou "REGISTO|username|password"
            networkStream.Read(protocolSI.Buffer, 0, protocolSI.Buffer.Length);
            string credBase64 = protocolSI.GetStringFromData();
            string credPlanas = GestorCriptografia.DecifrarAES(
                Convert.FromBase64String(credBase64), chaveAES, ivAES);

            string[] partes   = credPlanas.Split('|');
            string operacao   = partes.Length >= 3 ? partes[0] : "LOGIN";
            string password   = partes.Length >= 3 ? partes[2] : "";

            // PASSO 6: Executar Login ou Registo — valida com SHA-512 na BD
            bool sucesso;
            string msgResultado;

            if (operacao == "REGISTO")
            {
                sucesso = gestorUtilizadores.Registar(username, password);
                msgResultado = sucesso
                    ? $"'{username}' registado com sucesso."
                    : $"Registo falhado — username '{username}' já existe.";
            }
            else
            {
                sucesso = gestorUtilizadores.Autenticar(username, password);
                msgResultado = sucesso
                    ? $"'{username}' autenticado com sucesso."
                    : $"Autenticação falhada para '{username}'.";
            }

            Console.WriteLine("[{0}] {1}", DateTime.Now.ToLongTimeString(), msgResultado);

            if (!sucesso)
            {
                byte[] nack = protocolSI.Make(ProtocolSICmdType.NACK);
                lock (sendLock) { networkStream.Write(nack, 0, nack.Length); }
                logger.Warn(msgResultado);
                return false;
            }

            // PASSO 7: Sucesso — notificar restantes clientes e enviar ACK
            logger.Info(msgResultado);
            gestor.BroadcastMessage(this, username + " entrou no chat.");
            logger.Info($"'{username}' entrou no chat.");

            byte[] ack = protocolSI.Make(ProtocolSICmdType.ACK);
            lock (sendLock) { networkStream.Write(ack, 0, ack.Length); }
            return true;
        }

        /// <summary>
        /// Processa uma mensagem de chat recebida:
        /// decifra com a chave AES da sessão, regista no log e faz broadcast.
        /// </summary>
        private void HandleMensagem(ProtocolSI protocolSI)
        {
            if (chaveAES == null) return;

            string msgBase64 = protocolSI.GetStringFromData();
            string msgPlana  = GestorCriptografia.DecifrarAES(
                Convert.FromBase64String(msgBase64), chaveAES, ivAES);

            string displayName = username ?? ("Cliente" + clientID);
            Console.WriteLine("[{0}] {1}: {2}", DateTime.Now.ToLongTimeString(), displayName, msgPlana);
            logger.Info($"Mensagem de '{displayName}': {msgPlana}");

            gestor.BroadcastMessage(this, displayName + ": " + msgPlana);
        }

        /// <summary>
        /// Processa a desconexão do cliente e notifica os restantes.
        /// </summary>
        private void HandleDesconexao()
        {
            Console.WriteLine("[{0}] Cliente '{1}' desconectado.",
                DateTime.Now.ToLongTimeString(), username ?? "ID " + clientID);
            logger.Info($"Cliente '{username ?? "ID " + clientID}' desconectou-se.");

            if (username != null)
            {
                gestor.BroadcastMessage(this, username + " saiu do chat.");
                logger.Info($"'{username}' saiu do chat.");
            }
        }

        /// <summary>
        /// Cifra uma mensagem em texto simples com a chave AES desta sessão
        /// e envia-a para o cliente via ProtocolSI.
        /// </summary>
        public void EnviarMensagemCifrada(string mensagemPlana)
        {
            if (chaveAES == null) return;

            try
            {
                byte[] cifrado   = GestorCriptografia.CifrarAES(mensagemPlana, chaveAES, ivAES);
                string base64    = Convert.ToBase64String(cifrado);

                ProtocolSI proto = new ProtocolSI();
                byte[] packet    = proto.Make(ProtocolSICmdType.DATA, base64);
                lock (sendLock) { networkStream.Write(packet, 0, packet.Length); }
            }
            catch { }
        }
    }
}
