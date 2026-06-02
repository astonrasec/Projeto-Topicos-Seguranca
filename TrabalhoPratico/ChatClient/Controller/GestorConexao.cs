using EI.SI;
using System;
using System.Net;
using System.Net.Sockets;
using System.Security.Cryptography;

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

                // PASSO 1: Enviar username para o servidor
                byte[] packet = protocol.Make(ProtocolSICmdType.USER_OPTION_1, username);
                stream.Write(packet, 0, packet.Length);

                // PASSO 2: Receber chave pública do servidor
                stream.Read(protocol.Buffer, 0, protocol.Buffer.Length);
                string chavePublicaServidor = protocol.GetStringFromData();

                // PASSO 3: Gerar par de chaves RSA do cliente (padrão Ficha 5)
                using (RSACryptoServiceProvider rsa = new RSACryptoServiceProvider())
                {
                    SessaoAtual.ChavePrivada = rsa.ToXmlString(true); // Guardar chave privada
                    string chavePublicaCliente = rsa.ToXmlString(false); // Apenas pública

                    // Enviar chave pública do cliente para o servidor
                    byte[] clientKeyPacket = protocol.Make(ProtocolSICmdType.DATA, chavePublicaCliente);
                    stream.Write(clientKeyPacket, 0, clientKeyPacket.Length);
                }

                // PASSO 4: Aguardar confirmação (ACK)
                stream.Read(protocol.Buffer, 0, protocol.Buffer.Length);

                if (protocol.GetCmdType() == ProtocolSICmdType.ACK)
                {
                    SessaoAtual.ChavePublicaServidor = chavePublicaServidor;
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
