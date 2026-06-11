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

       
        public static bool TentarConectar(string username, string password, string ip,
            out TcpClient tcpClient, out NetworkStream stream, out ProtocolSI protocol, out SessaoAtual sessao)
        {
            return ExecutarHandshake("LOGIN", username, password, ip,
                out tcpClient, out stream, out protocol, out sessao);
        }


        public static bool TentarRegistar(string username, string password, string ip,
            out TcpClient tcpClient, out NetworkStream stream, out ProtocolSI protocol, out SessaoAtual sessao)
        {
            return ExecutarHandshake("REGISTO", username, password, ip,
                out tcpClient, out stream, out protocol, out sessao);
        }


        // cada chamada cria e devolve a sua própria SessaoAtual (out sessao),
        // em vez de escrever numa SessaoAtual estática partilhada por todo o processo.
        private static bool ExecutarHandshake(string operacao, string username, string password, string ip,
            out TcpClient tcpClient, out NetworkStream stream, out ProtocolSI protocol, out SessaoAtual sessao)
        {
            tcpClient = null;
            stream    = null;
            protocol  = null;
            sessao    = new SessaoAtual();

            try
            {
                IPEndPoint endpoint = new IPEndPoint(IPAddress.Parse(ip), PORT);
                tcpClient = new TcpClient();
                tcpClient.Connect(endpoint);

                stream   = tcpClient.GetStream();
                protocol = new ProtocolSI();

                // PASSO 1: Enviar username
                byte[] packet = protocol.Make(ProtocolSICmdType.USER_OPTION_1, username);
                stream.Write(packet, 0, packet.Length);

                // PASSO 2: Receber chave pública RSA do servidor
                stream.Read(protocol.Buffer, 0, protocol.Buffer.Length);
                string chavePublicaServidor = protocol.GetStringFromData();
                // CORREÇÃO: gravar na sessão local desta ligação, não num estático global
                sessao.ChavePublicaServidor = chavePublicaServidor;

                // PASSO 3: Gerar par de chaves RSA do cliente e enviar a chave pública
                string chavePrivada;
                string chavePublica;
                using (RSACryptoServiceProvider rsa = new RSACryptoServiceProvider())
                {
                    chavePrivada = rsa.ToXmlString(true);
                    chavePublica = rsa.ToXmlString(false);
                }
                //  gravar na sessão local desta ligação, não num estático global
                sessao.ChavePrivada = chavePrivada;

                byte[] packetPubKey = protocol.Make(ProtocolSICmdType.DATA, chavePublica);
                stream.Write(packetPubKey, 0, packetPubKey.Length);

                // PASSO 4: Receber chave AES cifrada com RSA (USER_OPTION_2)
                stream.Read(protocol.Buffer, 0, protocol.Buffer.Length);
                byte[] aesDataCifrado = Convert.FromBase64String(protocol.GetStringFromData());

                // Decifrar com chave privada RSA → chave (32 bytes) + IV (16 bytes)
                byte[] aesData  = GestorCriptografia.DecifrarComRSA(aesDataCifrado, chavePrivada);
                byte[] chaveAES = new byte[32];
                byte[] ivAES    = new byte[16];
                Buffer.BlockCopy(aesData, 0,  chaveAES, 0, 32);
                Buffer.BlockCopy(aesData, 32, ivAES,    0, 16);

                // chave AES/IV ficam na sessão local desta ligação
                // (cada cliente tem a sua, em vez de partilharem a mesma chave)
                sessao.ChaveAES = chaveAES;
                sessao.IVAES   = ivAES;

                
                string credenciais = operacao + "|" + username + "|" + password;
                string credBase64  = GestorCriptografia.CifrarMensagemAES(credenciais, chaveAES, ivAES);

                byte[] packetCred = protocol.Make(ProtocolSICmdType.USER_OPTION_3, credBase64);
                stream.Write(packetCred, 0, packetCred.Length);

                // PASSO 6: Aguardar ACK ou NACK
                stream.Read(protocol.Buffer, 0, protocol.Buffer.Length);

                if (protocol.GetCmdType() == ProtocolSICmdType.ACK)
                {
                    // gravar na sessão local desta ligação
                    sessao.Username = username;
                    sessao.IP       = ip;
                    return true;
                }

                // operação falhada
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
