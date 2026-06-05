using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace ChatClient
{
    /// <summary>
    /// Métodos de criptografia usados pelo cliente:
    /// - AES-256 CBC (simétrico): cifrar e decifrar mensagens de chat
    /// - RSA (assimétrico): decifrar a chave AES recebida do servidor
    /// </summary>
    public static class GestorCriptografia
    {
        private const int AES_KEY_SIZE   = 256;
        private const int AES_BLOCK_SIZE = 128;

        /// <summary>
        /// Cifra uma string com AES-256 CBC e devolve o resultado em Base64.
        /// Usado no envio de mensagens e credenciais de autenticação.
        /// </summary>
        public static string CifrarMensagemAES(string texto, byte[] chave, byte[] iv)
        {
            using (AesCryptoServiceProvider aes = new AesCryptoServiceProvider())
            {
                aes.KeySize   = AES_KEY_SIZE;
                aes.BlockSize = AES_BLOCK_SIZE;
                aes.Key       = chave;
                aes.IV        = iv;
                aes.Mode      = CipherMode.CBC;
                aes.Padding   = PaddingMode.PKCS7;

                using (ICryptoTransform enc = aes.CreateEncryptor())
                using (MemoryStream ms      = new MemoryStream())
                using (CryptoStream cs      = new CryptoStream(ms, enc, CryptoStreamMode.Write))
                {
                    byte[] dados = Encoding.UTF8.GetBytes(texto);
                    cs.Write(dados, 0, dados.Length);
                    cs.FlushFinalBlock();
                    return Convert.ToBase64String(ms.ToArray());
                }
            }
        }

        /// <summary>
        /// Decifra uma string Base64 que contém dados cifrados com AES-256 CBC.
        /// Usado na receção de mensagens de chat.
        /// </summary>
        public static string DecifrarMensagemAES(string base64Cifrado, byte[] chave, byte[] iv)
        {
            byte[] dadosCifrados = Convert.FromBase64String(base64Cifrado);

            using (AesCryptoServiceProvider aes = new AesCryptoServiceProvider())
            {
                aes.KeySize   = AES_KEY_SIZE;
                aes.BlockSize = AES_BLOCK_SIZE;
                aes.Key       = chave;
                aes.IV        = iv;
                aes.Mode      = CipherMode.CBC;
                aes.Padding   = PaddingMode.PKCS7;

                using (ICryptoTransform dec = aes.CreateDecryptor())
                using (MemoryStream ms      = new MemoryStream(dadosCifrados))
                using (CryptoStream cs      = new CryptoStream(ms, dec, CryptoStreamMode.Read))
                using (StreamReader sr      = new StreamReader(cs, Encoding.UTF8))
                {
                    return sr.ReadToEnd();
                }
            }
        }

        /// <summary>
        /// Decifra dados cifrados com RSA usando a chave privada em formato XML.
        /// Usado para recuperar a chave AES enviada pelo servidor.
        /// </summary>
        public static byte[] DecifrarComRSA(byte[] dadosCifrados, string chavePrivadaXml)
        {
            using (RSACryptoServiceProvider rsa = new RSACryptoServiceProvider())
            {
                rsa.FromXmlString(chavePrivadaXml);
                return rsa.Decrypt(dadosCifrados, false); // PKCS#1 v1.5
            }
        }
    }
}
