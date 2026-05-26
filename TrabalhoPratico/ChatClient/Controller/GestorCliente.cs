using System;

namespace ChatClient
{
    public static class GestorCliente
    {
        private static int activeClients = 0;
        private static Action<int> onClientCountChanged;

        public static event Action<int> OnClientCountChanged
        {
            add { onClientCountChanged += value; }
            remove { onClientCountChanged -= value; }
        }

        public static void RegisterClient()
        {
            activeClients++;
            onClientCountChanged?.Invoke(activeClients);
        }

        public static void UnregisterClient()
        {
            activeClients--;
            if (activeClients < 0) activeClients = 0;
            onClientCountChanged?.Invoke(activeClients);
        }

        public static int GetActiveClientCount()
        {
            return activeClients;
        }
    }
}
