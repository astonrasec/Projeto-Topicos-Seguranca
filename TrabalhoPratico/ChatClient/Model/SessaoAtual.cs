namespace ChatClient
{

    public class SessaoAtual
    {
        public string Username { get; set; }
        public string IP { get; set; }

        // Criptografia assimétrica (RSA) — troca de chaves com o servidor
        public string ChavePublicaServidor { get; set; }
        public string ChavePrivada { get; set; }

        // Criptografia simétrica (AES) — recebida do servidor cifrada com RSA
        public byte[] ChaveAES { get; set; }
        public byte[] IVAES   { get; set; }
    }
}
