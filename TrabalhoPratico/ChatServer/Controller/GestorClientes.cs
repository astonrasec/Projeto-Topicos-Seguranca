using EI.SI;
using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Threading;

namespace ChatServer
{
    public class GestorClientes
    {
        private readonly List<ClientHandler> clients = new List<ClientHandler>();
        private readonly object clientsLock = new object();
        private int clientCounter = 0;
        private readonly Logger logger;

        // RSA do servidor (Ficha 5 - Criptografia Assimétrica)
        private RSACryptoServiceProvider rsaServidor;
        public string ChavePublicaServidor { get; private set; }

        public GestorClientes(Logger logger)
        {
            this.logger = logger;

            // Gera o par de chaves RSA do servidor (padrão Ficha 5)
            rsaServidor = new RSACryptoServiceProvider();
            ChavePublicaServidor = rsaServidor.ToXmlString(false); // Apenas chave pública
            logger.Info("Chave RSA do servidor gerada com sucesso");
        }

        public ClientHandler AdicionarCliente(TcpClient tcpClient)
        {
            clientCounter++;
            int id = clientCounter;

            string msg = $"[{DateTime.Now.ToLongTimeString()}] Novo cliente conectado (ID: {id})";
            Console.WriteLine(msg);
            logger.Info($"Cliente ID:{id} conectado (IP: {tcpClient.Client.RemoteEndPoint})");

            ClientHandler handler = new ClientHandler(tcpClient, id, clients, clientsLock, this, logger);

            lock (clientsLock)
            {
                clients.Add(handler);
            }

            handler.Start();
            return handler;
        }

        public void RemoverCliente(ClientHandler handler)
        {
            lock (clientsLock)
            {
                clients.Remove(handler);
            }
        }

        public void BroadcastMessage(ClientHandler remetente, string message)
        {
            List<ClientHandler> snapshot;
            lock (clientsLock)
            {
                snapshot = new List<ClientHandler>(clients);
            }

            foreach (ClientHandler client in snapshot)
            {
                if (client != remetente)
                    client.SendMessage(message);
            }
        }
    }

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

        private readonly object sendLock = new object();

        // Chave pública do cliente (recebida durante o handshake RSA)
        public string ChavePublicaCliente { get; private set; }

        public ClientHandler(TcpClient client, int id, List<ClientHandler> clients, object lockObj, GestorClientes gestor, Logger logger)
        {
            this.tcpClient = client;
            this.clientID = id;
            this.allClients = clients;
            this.clientsLock = lockObj;
            this.gestor = gestor;
            this.logger = logger;
            this.networkStream = client.GetStream();
        }

        public void Start()
        {
            Thread thread = new Thread(Handle);
            thread.IsBackground = true;
            thread.Start();
        }

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
                            username = protocolSI.GetStringFromData();

                            Console.WriteLine("[{0}] Cliente {1} identificado como '{2}'",
                                DateTime.Now.ToLongTimeString(), clientID, username);

                            // PASSO 1: Enviar chave pública do servidor para o cliente
                            byte[] pubKeyPacket = protocolSI.Make(ProtocolSICmdType.DATA, gestor.ChavePublicaServidor);
                            lock (sendLock)
                            {
                                networkStream.Write(pubKeyPacket, 0, pubKeyPacket.Length);
                            }

                            // PASSO 2: Receber chave pública do cliente
                            networkStream.Read(protocolSI.Buffer, 0, protocolSI.Buffer.Length);
                            ChavePublicaCliente = protocolSI.GetStringFromData();

                            logger.Info($"Troca de chaves RSA concluída com '{username}'");

                            gestor.BroadcastMessage(this, username + " entrou no chat.");
                            logger.Info($"'{username}' entrou no chat");

                            // PASSO 3: Confirmar autenticação
                            byte[] ack = protocolSI.Make(ProtocolSICmdType.ACK);
                            lock (sendLock)
                            {
                                networkStream.Write(ack, 0, ack.Length);
                            }
                            break;

                        case ProtocolSICmdType.DATA:
                            string msg = protocolSI.GetStringFromData();
                            string displayName = username ?? ("Cliente" + clientID);

                            Console.WriteLine("[{0}] {1}: {2}",
                                DateTime.Now.ToLongTimeString(), displayName, msg);

                            logger.Info($"Mensagem de '{displayName}': {msg}");

                            gestor.BroadcastMessage(this, displayName + ": " + msg);
                            break;

                        case ProtocolSICmdType.EOT:
                            Console.WriteLine("[{0}] Cliente '{1}' desconectado.",
                                DateTime.Now.ToLongTimeString(), username ?? "ID " + clientID);

                            logger.Info($"Cliente '{username ?? "ID " + clientID}' desconectou-se");

                            if (username != null)
                            {
                                gestor.BroadcastMessage(this, username + " saiu do chat.");
                                logger.Info($"'{username}' saiu do chat");
                            }

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
                logger.Info($"Cliente ID:{clientID} recursos libertados");
            }
        }

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
