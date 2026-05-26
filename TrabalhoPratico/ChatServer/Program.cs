using System;
using System.Net;
using System.Net.Sockets;

namespace ChatServer
{
    class Program
    {
        private const int PORT = 10000;

        static void Main(string[] args)
        {
            IPEndPoint endpoint = new IPEndPoint(IPAddress.Any, PORT);
            TcpListener listener = new TcpListener(endpoint);
            listener.Start();

            Console.WriteLine("=== Servidor de Chat - Fase I ===");
            Console.WriteLine("Aberta a porta {0}...", PORT);
            Console.WriteLine("Aguardar ligações...");
            Console.WriteLine();

            GestorClientes gestor = new GestorClientes();

            while (true)
            {
                TcpClient tcpClient = listener.AcceptTcpClient();
                gestor.AdicionarCliente(tcpClient);
            }
        }
    }
}
