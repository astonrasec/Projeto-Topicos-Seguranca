using System;

namespace ChatClient
{
    /// <summary>
    /// Gerenciador centralizado de clientes ativos.
    /// 
    /// PROPÓSITO:
    ///   - Manter registro de quantos clientes estão conectados
    ///   - Notificar a interface quando o número de clientes muda
    ///   - Sincronizar estado entre FormLauncher e FormChat/FormLogin
    /// 
    /// PADRÃO:
    ///   - Classe estática singleton
    ///   - Usa event para notificações thread-safe
    ///   - Controlador de estado centralizado (single source of truth)
    /// 
    /// FLUXO:
    ///   1. FormLogin chama RegisterClient() quando conecta com sucesso
    ///   2. ClientManager incrementa contador e dispara evento
    ///   3. FormLauncher recebe evento e atualiza label
    ///   4. Quando FormChat fecha, FormLogin chama UnregisterClient()
    ///   5. ClientManager decrementa e dispara evento novamente
    /// </summary>
    public static class ClientManager
    {
        private static int activeClients = 0;
        private static Action<int> onClientCountChanged;

        /// <summary>
        /// Evento disparado sempre que o número de clientes ativos muda.
        /// Permite que múltiplas partes da aplicação fiquem sincronizadas.
        /// </summary>
        public static event Action<int> OnClientCountChanged
        {
            add { onClientCountChanged += value; }
            remove { onClientCountChanged -= value; }
        }

        /// <summary>
        /// Registra um novo cliente como ativo.
        /// Chamado quando FormLogin recebe ACK do servidor e abre FormChat.
        /// </summary>
        public static void RegisterClient()
        {
            activeClients++;
            onClientCountChanged?.Invoke(activeClients);
        }

        /// <summary>
        /// Desregistra um cliente (remove da contagem).
        /// Chamado quando FormChat fecha (via event handler em FormLogin).
        /// </summary>
        public static void UnregisterClient()
        {
            activeClients--;
            if (activeClients < 0) activeClients = 0;
            onClientCountChanged?.Invoke(activeClients);
        }

        /// <summary>
        /// Retorna o número atual de clientes conectados.
        /// Usado por FormLauncher para decidir se pode fechar ou não.
        /// </summary>
        public static int GetActiveClientCount()
        {
            return activeClients;
        }
    }
}
