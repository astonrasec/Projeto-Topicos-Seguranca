using EI.SI;
using System;
using System.Net;
using System.Net.Sockets;

namespace ChatClient
{
    public static class GestorConexao
    {
        public const int PORT = 10000;

        public static bool TentarConectar(string username, string ip, out TcpClient tcpClient, out NetworkStream stream, out ProtocolSI protocol)
        {
            tcpClient = null;
            stream = null;
            protocol = null;

            try
            {
                IPEndPoint endpoint = new IPEndPoint(IPAddress.Parse(ip), PORT);
                tcpClient = new TcpClient();
                tcpClient.Connect(endpoint);

                stream = tcpClient.GetStream();
                protocol = new ProtocolSI();

                byte[] packet = protocol.Make(ProtocolSICmdType.USER_OPTION_1, username);
                stream.Write(packet, 0, packet.Length);

                stream.Read(protocol.Buffer, 0, protocol.Buffer.Length);

                if (protocol.GetCmdType() == ProtocolSICmdType.ACK)
                {
                    return true;
                }

                stream.Close();
                tcpClient.Close();
                return false;
            }
            catch
            {
                tcpClient?.Close();
                stream?.Close();
                throw;
            }
        }
    }
}
