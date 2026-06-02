namespace ChatClient
{
    public static class SessaoAtual
    {
        public static string Username { get; set; }
        public static string IP { get; set; }

        // Chaves RSA (padrão Ficha 5)
        public static string ChavePublicaServidor { get; set; }
        public static string ChavePrivada { get; set; }
    }
}
