using EI.SI;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace ChatServer
{
    /// <summary>
    /// Classe principal do servidor de chat.
    /// Fica à escuta na porta 10000 e aceita múltiplos clientes em simultâneo,
    /// criando uma thread dedicada para cada ligação.
    /// </summary>
    class Program
    {
        private const int PORT = 10000;

        // Lista com todos os clientes atualmente conectados.
        // É partilhada entre threads, por isso o acesso é protegido por clientsLock.
        private static readonly List<ClientHandler> clients = new List<ClientHandler>();
        private static readonly object clientsLock = new object();

        static void Main(string[] args)
        {
            // Aceitar ligações de qualquer interface de rede da máquina
            IPEndPoint endpoint = new IPEndPoint(IPAddress.Any, PORT);
            TcpListener listener = new TcpListener(endpoint);
            listener.Start();

            Console.WriteLine("=== Servidor de Chat - Fase I ===");
            Console.WriteLine("Aberta a porta {0}...", PORT);
            Console.WriteLine("Aguardar ligações...");
            Console.WriteLine();

            int clientCounter = 0;

            // Ciclo principal: o servidor nunca termina, aceita clientes indefinidamente
            while (true)
            {
                // Bloqueia aqui até um cliente se ligar
                TcpClient tcpClient = listener.AcceptTcpClient();
                clientCounter++;

                Console.WriteLine("[{0}] Novo cliente conectado (ID: {1})",
                    DateTime.Now.ToLongTimeString(), clientCounter);

                ClientHandler handler = new ClientHandler(tcpClient, clientCounter, clients, clientsLock);

                lock (clientsLock)
                {
                    clients.Add(handler);
                }

                // Lança uma thread dedicada a este cliente e regressa imediatamente
                // ao ciclo para poder aceitar o próximo
                handler.Start();
            }
        }
    }

    /// <summary>
    /// Gere a comunicação com um cliente específico numa thread independente.
    /// Recebe mensagens do cliente e retransmite-as para todos os outros (broadcast).
    /// </summary>
    class ClientHandler
    {
        private readonly TcpClient tcpClient;
        private readonly NetworkStream networkStream;
        private readonly int clientID;
        private string username;

        // Referências partilhadas para poder fazer broadcast a todos os clientes
        private readonly List<ClientHandler> allClients;
        private readonly object clientsLock;

        // Lock individual por cliente: garante que duas threads não escrevem
        // ao mesmo tempo no mesmo NetworkStream (ex: broadcast simultâneo)
        private readonly object sendLock = new object();

        public ClientHandler(TcpClient client, int id, List<ClientHandler> clients, object lockObj)
        {
            this.tcpClient    = client;
            this.clientID     = id;
            this.allClients   = clients;
            this.clientsLock  = lockObj;
            this.networkStream = client.GetStream();
        }

        /// <summary>
        /// Cria e inicia a thread de background dedicada a este cliente.
        /// IsBackground = true garante que a thread termina quando o processo terminar.
        /// </summary>
        public void Start()
        {
            Thread thread = new Thread(Handle);
            thread.IsBackground = true;
            thread.Start();
        }

        /// <summary>
        /// Método principal da thread. Lê mensagens do cliente em ciclo
        /// e trata cada tipo de pacote recebido (USER_OPTION_1, DATA, EOT).
        /// Termina quando o cliente envia EOT, fecha a ligação ou ocorre um erro.
        /// O bloco finally garante que os recursos são sempre libertados.
        /// </summary>
        private void Handle()
        {
            ProtocolSI protocolSI = new ProtocolSI();

            try
            {
                while (true)
                {
                    int bytesRead = networkStream.Read(protocolSI.Buffer, 0, protocolSI.Buffer.Length);

                    // 0 bytes significa que o cliente fechou a ligação abruptamente
                    if (bytesRead == 0) break;

                    switch (protocolSI.GetCmdType())
                    {
                        case ProtocolSICmdType.USER_OPTION_1:
                            // O cliente envia o seu username assim que se liga.
                            // Guardamos o nome, notificamos os outros e respondemos com ACK.
                            username = protocolSI.GetStringFromData();
                            Console.WriteLine("[{0}] Cliente {1} identificado como '{2}'",
                                DateTime.Now.ToLongTimeString(), clientID, username);

                            BroadcastMessage(username + " entrou no chat.");

                            byte[] ack = protocolSI.Make(ProtocolSICmdType.ACK);
                            lock (sendLock)
                            {
                                networkStream.Write(ack, 0, ack.Length);
                            }
                            break;

                        case ProtocolSICmdType.DATA:
                            // Mensagem de chat: mostrar na consola e reencaminhar para os outros clientes
                            string msg = protocolSI.GetStringFromData();
                            string displayName = username ?? ("Cliente" + clientID);

                            Console.WriteLine("[{0}] {1}: {2}",
                                DateTime.Now.ToLongTimeString(), displayName, msg);

                            BroadcastMessage(displayName + ": " + msg);
                            break;

                        case ProtocolSICmdType.EOT:
                            // O cliente pediu desconexão de forma limpa
                            Console.WriteLine("[{0}] Cliente '{1}' desconectado.",
                                DateTime.Now.ToLongTimeString(), username ?? "ID " + clientID);

                            if (username != null)
                                BroadcastMessage(username + " saiu do chat.");

                            return;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("[Erro] Cliente {0}: {1}", clientID, ex.Message);
            }
            finally
            {
                // Sempre executado — remove o cliente da lista e liberta os recursos
                lock (clientsLock)
                {
                    allClients.Remove(this);
                }
                networkStream.Close();
                tcpClient.Close();
            }
        }

        /// <summary>
        /// Envia uma mensagem a todos os clientes conectados, exceto ao remetente.
        /// Copia a lista antes de iterar para evitar conflitos com Add/Remove
        /// que possam ocorrer noutras threads durante o envio.
        /// </summary>
        private void BroadcastMessage(string message)
        {
            List<ClientHandler> snapshot;
            lock (clientsLock)
            {
                snapshot = new List<ClientHandler>(allClients);
            }

            foreach (ClientHandler client in snapshot)
            {
                if (client != this)
                    client.SendMessage(message);
            }
        }

        /// <summary>
        /// Envia um pacote DATA a este cliente específico.
        /// Usa sendLock para evitar escritas simultâneas no mesmo NetworkStream.
        /// Erros são ignorados silenciosamente — o cliente pode ter desconectado entretanto.
        /// </summary>
        public void SendMessage(string message)
        {
            try
            {
                ProtocolSI proto = new ProtocolSI();
                byte[] packet = proto.Make(ProtocolSICmdType.DATA, message);
                lock (sendLock)
                {
                    networkStream.Write(packet, 0, packet.Length);
                }
            }
            catch { }
        }
    }
}
