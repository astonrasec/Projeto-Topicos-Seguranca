using EI.SI;
using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Threading;

namespace ChatServer
{
    public class GestorClientes
    {
        private readonly List<ClientHandler> clients = new List<ClientHandler>();
        private readonly object clientsLock = new object();
        private int clientCounter = 0;

        public ClientHandler AdicionarCliente(TcpClient tcpClient)
        {
            clientCounter++;
            int id = clientCounter;

            Console.WriteLine("[{0}] Novo cliente conectado (ID: {1})",
                DateTime.Now.ToLongTimeString(), id);

            ClientHandler handler = new ClientHandler(tcpClient, id, clients, clientsLock, this);

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

        private readonly object sendLock = new object();

        public ClientHandler(TcpClient client, int id, List<ClientHandler> clients, object lockObj, GestorClientes gestor)
        {
            this.tcpClient = client;
            this.clientID = id;
            this.allClients = clients;
            this.clientsLock = lockObj;
            this.gestor = gestor;
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

                            gestor.BroadcastMessage(this, username + " entrou no chat.");

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

                            gestor.BroadcastMessage(this, displayName + ": " + msg);
                            break;

                        case ProtocolSICmdType.EOT:
                            Console.WriteLine("[{0}] Cliente '{1}' desconectado.",
                                DateTime.Now.ToLongTimeString(), username ?? "ID " + clientID);

                            if (username != null)
                                gestor.BroadcastMessage(this, username + " saiu do chat.");

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
                gestor.RemoverCliente(this);
                networkStream.Close();
                tcpClient.Close();
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
