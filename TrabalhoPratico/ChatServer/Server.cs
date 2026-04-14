// ============================================================
// BIBLIOTECAS / NAMESPACES UTILIZADOS
// ============================================================
// EI.SI      -> biblioteca ProtocolSI.dll fornecida pelo docente.
//              Define o protocolo de comunicação (tipos de mensagem,
//              serialização de dados para bytes, etc.)
// System.Net -> contém IPAddress, IPEndPoint (endereço IP + porta)
// System.Net.Sockets -> contém TcpListener e TcpClient (sockets TCP)
// System.Threading   -> contém Thread (execução paralela)
// System.Collections.Generic -> contém List<T>
// ============================================================
using EI.SI;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace ChatServer
{
    // ============================================================
    // CONCEITO: O que é um servidor TCP?
    //
    // Um servidor TCP fica "à escuta" numa porta específica.
    // Quando um cliente se liga, o servidor cria uma ligação
    // dedicada (socket) com esse cliente.
    // Para suportar vários clientes ao mesmo tempo, o servidor
    // cria uma Thread separada para cada cliente — assim cada
    // um é tratado de forma independente e em paralelo.
    // ============================================================

    /// <summary>
    /// Classe principal do servidor de chat.
    ///
    /// RESPONSABILIDADE:
    ///   - Iniciar o TcpListener na porta 10000
    ///   - Aceitar ligações de clientes num ciclo infinito
    ///   - Para cada cliente que se liga, criar um ClientHandler
    ///     e lançar uma Thread para o gerir
    ///
    /// FASE I: sem cifragem — as mensagens viajam em texto simples.
    /// </summary>
    class Program
    {
        // --------------------------------------------------------
        // PORTA TCP: número que identifica o "canal" de comunicação.
        // O cliente e o servidor têm de usar a mesma porta.
        // Portas < 1024 são reservadas pelo sistema; usamos 10000.
        // --------------------------------------------------------
        private const int PORT = 10000;

        // --------------------------------------------------------
        // LISTA DE CLIENTES CONECTADOS
        //
        // Quando um cliente se liga, o seu ClientHandler é adicionado
        // aqui. Quando se desliga, é removido.
        // Esta lista é partilhada por TODAS as threads — por isso
        // precisamos de um mecanismo de sincronização (clientsLock).
        // --------------------------------------------------------
        private static readonly List<ClientHandler> clients = new List<ClientHandler>();

        // --------------------------------------------------------
        // LOCK (Mutex implícito em C#)
        //
        // Em C#, "lock(obj) { ... }" garante que só UMA thread de
        // cada vez pode executar o bloco protegido.
        // Sem isto, duas threads podiam modificar 'clients' ao mesmo
        // tempo, corrompendo a lista (race condition).
        // --------------------------------------------------------
        private static readonly object clientsLock = new object();

        /// <summary>
        /// Ponto de entrada do servidor.
        /// Cria o TcpListener, inicia-o e entra no ciclo de aceitação
        /// de clientes.
        /// </summary>
        static void Main(string[] args)
        {
            // --------------------------------------------------------
            // IPEndPoint: combina um endereço IP com uma porta.
            // IPAddress.Any = aceitar ligações de QUALQUER interface
            //                 de rede da máquina (localhost, LAN, etc.)
            // --------------------------------------------------------
            IPEndPoint endpoint = new IPEndPoint(IPAddress.Any, PORT);

            // --------------------------------------------------------
            // TcpListener: "ouve" ligações TCP na porta definida.
            // É o equivalente a colocar um rececionista na porta.
            // listener.Start() → abre a porta e começa a escutar.
            // --------------------------------------------------------
            TcpListener listener = new TcpListener(endpoint);
            listener.Start();

            Console.WriteLine("=== Servidor de Chat - Fase I ===");
            Console.WriteLine("À escuta na porta {0}...", PORT);
            Console.WriteLine("Aguardando ligações de clientes...");
            Console.WriteLine();

            // Contador para dar um ID único a cada cliente
            int clientCounter = 0;

            // --------------------------------------------------------
            // CICLO PRINCIPAL DO SERVIDOR (infinito)
            //
            // O servidor nunca termina sozinho — fica sempre à espera
            // de novos clientes. Para parar, fecha-se a janela/consola.
            // --------------------------------------------------------
            while (true)
            {
                // --------------------------------------------------------
                // AcceptTcpClient() BLOQUEIA aqui até que um cliente
                // se ligue. Quando um cliente aparece, retorna um
                // TcpClient com a ligação já estabelecida.
                //
                // É como um rececionista que fica parado à espera que
                // alguém entre pela porta.
                // --------------------------------------------------------
                TcpClient tcpClient = listener.AcceptTcpClient();
                clientCounter++;

                Console.WriteLine("[{0}] Novo cliente conectado (ID: {1})",
                    DateTime.Now.ToLongTimeString(), clientCounter);

                // --------------------------------------------------------
                // CRIAR O HANDLER para este cliente.
                // Passamos:
                //   - tcpClient: a ligação com este cliente específico
                //   - clientCounter: ID único
                //   - clients: lista partilhada (para poder fazer broadcast)
                //   - clientsLock: lock da lista partilhada
                // --------------------------------------------------------
                ClientHandler handler = new ClientHandler(tcpClient, clientCounter, clients, clientsLock);

                // Adicionar à lista de clientes ativos (protegido por lock)
                lock (clientsLock)
                {
                    clients.Add(handler);
                }

                // --------------------------------------------------------
                // LANÇAR THREAD para este cliente.
                // O Start() cria uma Thread que corre o método Handle()
                // independentemente. O ciclo while(true) do Main
                // pode imediatamente aceitar o próximo cliente.
                //
                // SEM THREADS: o servidor ficaria bloqueado a tratar
                // o Cliente1 e não conseguia aceitar o Cliente2.
                // --------------------------------------------------------
                handler.Start();
            }
        }
    }

    // ============================================================
    // CONCEITO: Por que precisamos da classe ClientHandler?
    //
    // Cada cliente tem a sua própria ligação TCP (NetworkStream),
    // o seu próprio username, e a sua própria Thread de leitura.
    // A classe ClientHandler agrupa tudo o que pertence a UM cliente.
    // ============================================================

    /// <summary>
    /// Gere a comunicação com um cliente específico.
    ///
    /// RESPONSABILIDADE:
    ///   - Correr numa Thread dedicada (um por cliente)
    ///   - Ler mensagens enviadas pelo cliente
    ///   - Processar cada tipo de mensagem (USER_OPTION_1, DATA, EOT)
    ///   - Fazer broadcast das mensagens para todos os outros clientes
    ///   - Limpar os recursos ao desligar
    /// </summary>
    class ClientHandler
    {
        // --------------------------------------------------------
        // LIGAÇÃO COM ESTE CLIENTE ESPECÍFICO
        //
        // tcpClient   → representa a ligação TCP
        // networkStream → canal de leitura/escrita de bytes
        //
        // Cada cliente tem o SEU networkStream.
        // Escrever neste stream envia dados APENAS para este cliente.
        // --------------------------------------------------------
        private readonly TcpClient tcpClient;
        private readonly NetworkStream networkStream;

        // ID numérico (1, 2, 3...) atribuído quando o cliente se liga
        private readonly int clientID;

        // Nome de utilizador — preenchido depois de receber USER_OPTION_1
        private string username;

        // --------------------------------------------------------
        // REFERÊNCIAS PARTILHADAS
        //
        // allClients → lista com TODOS os clientes conectados.
        //              Usada para fazer broadcast (enviar a todos).
        // clientsLock → lock para aceder a allClients em segurança.
        // --------------------------------------------------------
        private readonly List<ClientHandler> allClients;
        private readonly object clientsLock;

        // --------------------------------------------------------
        // LOCK DE ESCRITA DESTE CLIENTE
        //
        // Vários clientes podem querer enviar mensagens A ESTE
        // cliente ao mesmo tempo (via broadcast de threads diferentes).
        // O sendLock garante que só UM escreve de cada vez no stream,
        // evitando que os bytes se misturem.
        // --------------------------------------------------------
        private readonly object sendLock = new object();

        /// <summary>
        /// Construtor: guarda todas as referências necessárias
        /// e obtém o NetworkStream da ligação TCP.
        /// </summary>
        public ClientHandler(TcpClient client, int id, List<ClientHandler> clients, object lockObj)
        {
            this.tcpClient    = client;
            this.clientID     = id;
            this.allClients   = clients;
            this.clientsLock  = lockObj;

            // GetStream() devolve o canal bidirecional de comunicação.
            // É através deste stream que lemos e escrevemos bytes.
            this.networkStream = client.GetStream();
        }

        /// <summary>
        /// Cria e inicia a Thread dedicada a este cliente.
        ///
        /// IsBackground = true → a thread termina automaticamente
        /// quando o programa principal fechar, sem bloquear o fecho.
        /// </summary>
        public void Start()
        {
            Thread thread = new Thread(Handle);
            thread.IsBackground = true;
            thread.Start();
            // Após Start(), o Main() pode continuar e aceitar o próximo cliente
        }

        /// <summary>
        /// MÉTODO PRINCIPAL DA THREAD.
        ///
        /// Corre num ciclo infinito enquanto o cliente estiver conectado.
        /// Lê cada mensagem recebida, identifica o seu tipo pelo
        /// ProtocolSI e toma a ação correspondente.
        ///
        /// Termina quando:
        ///   - Recebe EOT (cliente pediu para desligar)
        ///   - bytesRead == 0 (ligação fechada abruptamente)
        ///   - Ocorre uma exceção (ex: cabo de rede desligado)
        /// </summary>
        private void Handle()
        {
            // --------------------------------------------------------
            // ProtocolSI: biblioteca que define o formato dos pacotes.
            // Cada pacote tem um cabeçalho (tipo de mensagem) + dados.
            //
            // protocolSI.Buffer → array de bytes onde os dados chegam.
            // protocolSI.GetCmdType() → identifica o tipo do pacote.
            // protocolSI.GetStringFromData() → extrai o texto do pacote.
            // protocolSI.Make(tipo, texto) → cria um pacote para enviar.
            // --------------------------------------------------------
            ProtocolSI protocolSI = new ProtocolSI();

            try
            {
                // Ciclo de leitura: lê continuamente do cliente
                while (true)
                {
                    // --------------------------------------------------------
                    // networkStream.Read() BLOQUEIA até chegarem dados.
                    // Quando chegam, preenche protocolSI.Buffer com os bytes
                    // e devolve quantos bytes foram lidos.
                    // --------------------------------------------------------
                    int bytesRead = networkStream.Read(
                        protocolSI.Buffer, 0, protocolSI.Buffer.Length);

                    // Se o servidor leu 0 bytes, significa que o cliente
                    // fechou a ligação sem enviar EOT (ex: programa fechado
                    // à força). Saímos do ciclo.
                    if (bytesRead == 0) break;

                    // Verificar o TIPO de mensagem recebida e agir em conformidade
                    switch (protocolSI.GetCmdType())
                    {
                        // ------------------------------------------------
                        // USER_OPTION_1: mensagem de identificação.
                        // O cliente envia esta mensagem logo ao ligar,
                        // contendo o seu nome de utilizador.
                        //
                        // FLUXO:
                        //   1. Guardar o username
                        //   2. Informar os outros clientes que este entrou
                        //   3. Enviar ACK ao cliente (confirmação de aceite)
                        // ------------------------------------------------
                        case ProtocolSICmdType.USER_OPTION_1:
                            username = protocolSI.GetStringFromData();
                            Console.WriteLine("[{0}] Cliente {1} identificado como '{2}'",
                                DateTime.Now.ToLongTimeString(), clientID, username);

                            // Broadcast de entrada (para TODOS EXCETO este)
                            BroadcastMessage(username + " entrou no chat.");

                            // ACK = Acknowledgment (confirmação).
                            // Diz ao cliente: "recebi o teu username, podes entrar".
                            byte[] ack = protocolSI.Make(ProtocolSICmdType.ACK);
                            lock (sendLock)
                            {
                                networkStream.Write(ack, 0, ack.Length);
                            }
                            break;

                        // ------------------------------------------------
                        // DATA: mensagem de chat normal.
                        // O cliente enviou texto que deve ser retransmitido
                        // a todos os outros clientes conectados.
                        //
                        // FLUXO:
                        //   1. Extrair o texto da mensagem
                        //   2. Mostrar na consola do servidor (log)
                        //   3. Fazer broadcast com prefixo "username: texto"
                        // ------------------------------------------------
                        case ProtocolSICmdType.DATA:
                            string msg = protocolSI.GetStringFromData();

                            // Usar username se conhecido, senão usar "ClienteX"
                            string displayName = username ?? ("Cliente" + clientID);

                            Console.WriteLine("[{0}] {1}: {2}",
                                DateTime.Now.ToLongTimeString(), displayName, msg);

                            // Retransmitir para todos os outros clientes
                            // O remetente já viu a sua própria mensagem no cliente
                            BroadcastMessage(displayName + ": " + msg);
                            break;

                        // ------------------------------------------------
                        // EOT (End of Transmission): pedido de desconexão.
                        // O cliente enviou este tipo quando fechou a janela
                        // ou clicou em "Desligar".
                        //
                        // FLUXO:
                        //   1. Informar os outros clientes que este saiu
                        //   2. Sair do ciclo (return → vai para o finally)
                        // ------------------------------------------------
                        case ProtocolSICmdType.EOT:
                            Console.WriteLine("[{0}] Cliente '{1}' desconectado.",
                                DateTime.Now.ToLongTimeString(), username ?? "ID " + clientID);

                            // Notificar os outros que este utilizador saiu
                            if (username != null)
                                BroadcastMessage(username + " saiu do chat.");

                            return; // Sair do método → vai para o bloco finally
                    }
                }
            }
            catch (Exception ex)
            {
                // Exceção = ligação interrompida inesperadamente
                Console.WriteLine("[Erro] Cliente {0}: {1}", clientID, ex.Message);
            }
            finally
            {
                // --------------------------------------------------------
                // BLOCO FINALLY: executado SEMPRE, seja por return, break
                // ou exceção. Garante que a limpeza é sempre feita.
                //
                // 1. Remover da lista partilhada (com lock, pois é partilhada)
                // 2. Fechar o stream e a ligação TCP → libertar recursos
                // --------------------------------------------------------
                lock (clientsLock)
                {
                    allClients.Remove(this);
                }
                networkStream.Close();
                tcpClient.Close();
            }
        }

        /// <summary>
        /// Envia uma mensagem para TODOS os clientes conectados,
        /// EXCETO o remetente (this).
        ///
        /// PORQUÊ tirar um "snapshot" da lista?
        ///   Não podemos percorrer a lista enquanto outras threads
        ///   a modificam (Add/Remove). Ao copiar para uma nova lista,
        ///   fazemos o percurso na cópia — seguro e sem lock prolongado.
        /// </summary>
        /// <param name="message">Texto a enviar a todos os outros clientes</param>
        private void BroadcastMessage(string message)
        {
            // Copiar a lista rapidamente com lock
            List<ClientHandler> snapshot;
            lock (clientsLock)
            {
                // new List<>(allClients) cria uma cópia independente
                snapshot = new List<ClientHandler>(allClients);
            }
            // Agora percorremos sem lock — a lista original pode mudar,
            // mas a cópia (snapshot) é estável

            foreach (ClientHandler client in snapshot)
            {
                // Não enviar ao remetente — ele já viu a sua própria mensagem
                if (client != this)
                {
                    client.SendMessage(message);
                }
            }
        }

        /// <summary>
        /// Envia uma mensagem (pacote DATA) para ESTE cliente específico.
        ///
        /// É chamado pelo BroadcastMessage de OUTRA thread.
        /// O sendLock garante que duas threads não escrevem
        /// simultaneamente neste NetworkStream.
        ///
        /// O try/catch silencia erros — se o cliente se desligou
        /// entretanto, simplesmente ignoramos o erro de envio.
        /// </summary>
        /// <param name="message">Texto a enviar</param>
        public void SendMessage(string message)
        {
            try
            {
                // Criar uma nova instância de ProtocolSI apenas para
                // esta mensagem (evita conflito com a instância do Handle)
                ProtocolSI proto = new ProtocolSI();
                byte[] packet = proto.Make(ProtocolSICmdType.DATA, message);

                // Lock: só uma thread escreve neste stream de cada vez
                lock (sendLock)
                {
                    networkStream.Write(packet, 0, packet.Length);
                }
            }
            catch
            {
                // O cliente pode ter desconectado entre o BroadcastMessage
                // e este envio — não é um erro crítico, ignoramos.
            }
        }
    }
}
