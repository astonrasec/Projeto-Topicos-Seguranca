namespace ChatClient
{
    
    /// Armazena os dados da sessão atual do cliente:
    /// credenciais de login, chaves RSA e chave AES para comunicação cifrada.
   
    public static class SessaoAtual
    {
        public static string Username { get; set; }
        public static string IP { get; set; }

        // Criptografia assimétrica (RSA) — troca de chaves com o servidor
        public static string ChavePublicaServidor { get; set; }
        public static string ChavePrivada { get; set; }

        // Criptografia simétrica (AES) — recebida do servidor cifrada com RSA
        public static byte[] ChaveAES { get; set; }
        public static byte[] IVAES   { get; set; }
    }
}
