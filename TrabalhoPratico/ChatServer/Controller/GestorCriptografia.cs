using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace ChatServer
{
    /// <summary>
    /// Métodos de criptografia usados pelo servidor:
    /// - AES-256 CBC (simétrico): cifrar e decifrar mensagens
    /// - RSA (assimétrico): cifrar a chave AES com a chave pública do cliente
    /// - SHA-512 + salt: hash de passwords para autenticação
    /// </summary>
    public static class GestorCriptografia
    {
        private const int AES_KEY_SIZE   = 256;
        private const int AES_BLOCK_SIZE = 128;

        /// <summary>
        /// Gera uma chave AES-256 e um IV de 128 bits aleatórios.
        /// Chave: 32 bytes | IV: 16 bytes
        /// </summary>
        public static void GerarChaveAES(out byte[] chave, out byte[] iv)
        {
            using (AesCryptoServiceProvider aes = new AesCryptoServiceProvider())
            {
                aes.KeySize   = AES_KEY_SIZE;
                aes.BlockSize = AES_BLOCK_SIZE;
                aes.GenerateKey();
                aes.GenerateIV();
                chave = aes.Key;
                iv    = aes.IV;
            }
        }

        /// <summary>
        /// Cifra uma string com AES-256 em modo CBC e PKCS7.
        /// Devolve os bytes cifrados.
        /// </summary>
        public static byte[] CifrarAES(string texto, byte[] chave, byte[] iv)
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
                    return ms.ToArray();
                }
            }
        }

        /// <summary>
        /// Decifra bytes cifrados com AES-256 CBC.
        /// Devolve a string original em UTF-8.
        /// </summary>
        public static string DecifrarAES(byte[] dadosCifrados, byte[] chave, byte[] iv)
        {
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
        /// Cifra dados binários com a chave pública RSA fornecida em XML.
        /// Usado para cifrar a chave AES antes de enviar ao cliente.
        /// </summary>
        public static byte[] CifrarComRSA(byte[] dados, string chavePublicaXml)
        {
            using (RSACryptoServiceProvider rsa = new RSACryptoServiceProvider())
            {
                rsa.FromXmlString(chavePublicaXml);
                return rsa.Encrypt(dados, false); // PKCS#1 v1.5
            }
        }

        /// <summary>
        /// Gera um salt aleatório de 16 bytes codificado em Base64.
        /// Cada utilizador tem um salt único para evitar ataques de dicionário.
        /// </summary>
        public static string GerarSalt()
        {
            byte[] saltBytes = new byte[16];
            using (RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider())
            {
                rng.GetBytes(saltBytes);
            }
            return Convert.ToBase64String(saltBytes);
        }

        /// <summary>
        /// Calcula o hash SHA-512 da combinação salt + password.
        /// Devolve o resultado em hexadecimal (128 caracteres).
        /// </summary>
        public static string HashPassword(string password, string salt)
        {
            byte[] input = Encoding.UTF8.GetBytes(salt + password);

            using (SHA512 sha512 = SHA512.Create())
            {
                byte[] hashBytes = sha512.ComputeHash(input);
                StringBuilder sb = new StringBuilder();
                foreach (byte b in hashBytes)
                    sb.Append(b.ToString("x2"));
                return sb.ToString();
            }
        }
    }
}
