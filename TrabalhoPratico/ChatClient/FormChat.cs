// ============================================================
// BIBLIOTECAS / NAMESPACES UTILIZADOS
// ============================================================
// EI.SI              → ProtocolSI (protocolo de comunicação)
// System.Net.Sockets → TcpClient, NetworkStream
// System.Threading   → Thread (thread de receção em background)
// System.Windows.Forms → Form, RichTextBox, Button, etc.
// ============================================================
using EI.SI;
using System;
using System.Net.Sockets;
using System.Threading;
using System.Windows.Forms;

namespace ChatClient
{
    // ============================================================
    // CONCEITO: Threads no Cliente
    //
    // O cliente precisa de fazer DUAS coisas ao mesmo tempo:
    //   1. Esperar que o utilizador escreva e clique "Enviar" (UI thread)
    //   2. Receber mensagens que chegam do servidor a qualquer momento
    //
    // Se usarmos uma só thread para ambas, a UI ficaria bloqueada
    // enquanto esperava por mensagens do servidor.
    //
    // SOLUÇÃO: criar uma SEGUNDA THREAD (receiveThread) que fica
    // em segundo plano (background) continuamente a ler o stream.
    // A thread da UI continua responsiva para o utilizador.
    //
    // PROBLEMA: ambas as threads partilham o networkStream e o
    // RichTextBox → precisamos de mecanismos de sincronização.
    // ============================================================

    /// <summary>
    /// Formulário principal do Chat — janela de conversação.
    ///
    /// RESPONSABILIDADE:
    ///   - Mostrar o histórico de mensagens (RichTextBox)
    ///   - Permitir ao utilizador escrever e enviar mensagens
    ///   - Receber mensagens do servidor em background (Thread separada)
    ///   - Fechar a ligação de forma limpa ao sair
    /// </summary>
    public partial class FormChat : Form
    {
        // --------------------------------------------------------
        // LIGAÇÃO AO SERVIDOR
        //
        // tcpClient     → a ligação TCP (usado para fechar no fim)
        // networkStream → canal bidirecional de leitura/escrita
        //                 É o mesmo stream aberto no FormLogin.
        // --------------------------------------------------------
        private readonly TcpClient tcpClient;
        private readonly NetworkStream networkStream;

        // --------------------------------------------------------
        // PROTOCOLO DE ENVIO
        //
        // Usado APENAS para construir e enviar pacotes (escrita).
        // Corre na thread da UI (botão Enviar, botão Desligar).
        // --------------------------------------------------------
        private readonly ProtocolSI sendProtocol;

        // Nome do utilizador (para mostrar na interface)
        private readonly string username;

        // --------------------------------------------------------
        // THREAD DE RECEÇÃO
        //
        // Thread dedicada a ler mensagens do servidor continuamente.
        // Corre em background — não bloqueia a interface gráfica.
        // --------------------------------------------------------
        private Thread receiveThread;

        // --------------------------------------------------------
        // FLAG DE CONTROLO DA THREAD (volatile)
        //
        // 'volatile' garante que quando uma thread altera 'running',
        // as outras threads veem o valor atualizado imediatamente.
        // Sem 'volatile', o compilador poderia fazer cache do valor
        // e a thread de receção nunca saberia que deve parar.
        // --------------------------------------------------------
        private volatile bool running = true;

        // --------------------------------------------------------
        // FLAG DE DESCONEXÃO
        //
        // Evita que o código de desconexão seja executado duas vezes.
        // Cenário: o utilizador clica "Desligar" (chama Disconnect())
        // e depois a janela fecha (FormClosing chama Disconnect() de novo).
        // --------------------------------------------------------
        private bool isDisconnecting = false;

        // --------------------------------------------------------
        // LOCK DE ESCRITA NO STREAM
        //
        // Garante que só UMA thread escreve no networkStream de cada vez.
        //
        // PORQUÊ é necessário?
        //   - A thread da UI escreve quando o utilizador envia mensagem
        //   - A thread da UI também escreve o EOT ao desligar
        //   → Se duas threads tentassem escrever ao mesmo tempo,
        //     os bytes poderiam misturar-se, corrompendo o pacote.
        //
        // NOTA: Ler (receiveThread) e Escrever (UI thread) podem
        //       acontecer em simultâneo num NetworkStream — é seguro.
        //       Só Escrever + Escrever em simultâneo é problemático.
        // --------------------------------------------------------
        private readonly object sendLock = new object();

        /// <summary>
        /// Construtor do FormChat.
        ///
        /// Recebe a ligação já estabelecida pelo FormLogin — não cria
        /// uma nova ligação, continua a usar a mesma.
        ///
        /// Após inicializar a interface, lança imediatamente a thread
        /// de receção para começar a receber mensagens.
        /// </summary>
        /// <param name="client">TcpClient com a ligação ao servidor</param>
        /// <param name="stream">NetworkStream da ligação</param>
        /// <param name="protocol">Instância ProtocolSI para envio</param>
        /// <param name="username">Nome de utilizador autenticado</param>
        public FormChat(TcpClient client, NetworkStream stream, ProtocolSI protocol, string username)
        {
            // Inicializa todos os controlos gráficos (Designer.cs)
            InitializeComponent();

            // Guardar referências para usar nos métodos seguintes
            this.tcpClient    = client;
            this.networkStream = stream;
            this.sendProtocol = protocol;
            this.username     = username;

            // Atualizar a interface com o nome do utilizador
            this.Text         = "Chat - " + username;          // Título da janela
            labelStatus.Text  = "Nome de utilizador: " + username; // Etiqueta de estado

            // Mensagem de boas-vindas no chat
            AppendMessage("=== Bem-vindo ao Chat, " + username + "! ===");

            // --------------------------------------------------------
            // LANÇAR A THREAD DE RECEÇÃO
            //
            // new Thread(ReceiveMessages) → cria uma thread que vai
            //   executar o método ReceiveMessages() quando iniciada.
            //
            // IsBackground = true → a thread é de background.
            //   Isto significa que quando a janela fechar e o programa
            //   terminar, esta thread é terminada automaticamente.
            //   Sem isto, o programa ficaria "vivo" em memória mesmo
            //   depois de todas as janelas serem fechadas.
            //
            // Start() → inicia a execução da thread.
            //   A partir daqui, ReceiveMessages() corre em paralelo
            //   com o resto da aplicação.
            // --------------------------------------------------------
            receiveThread = new Thread(ReceiveMessages);
            receiveThread.IsBackground = true;
            receiveThread.Start();
        }

        // ============================================================
        // THREAD DE RECEÇÃO
        // ============================================================

        /// <summary>
        /// Corre em background e lê continuamente mensagens do servidor.
        ///
        /// PORQUÊ uma instância SEPARADA de ProtocolSI (recvProtocol)?
        ///
        ///   O ProtocolSI tem um Buffer interno onde os dados são lidos.
        ///   Se usássemos o mesmo 'sendProtocol' aqui, enquanto esta
        ///   thread lê para o Buffer, a thread da UI poderia estar a
        ///   usar o mesmo Buffer para enviar — os dados misturar-se-iam.
        ///
        ///   Com instâncias separadas:
        ///     - recvProtocol.Buffer → usado só para LEITURA (esta thread)
        ///     - sendProtocol.Buffer → usado só para ESCRITA (UI thread)
        ///   → Sem conflito.
        /// </summary>
        private void ReceiveMessages()
        {
            // Instância dedicada para a receção — independente do sendProtocol
            ProtocolSI recvProtocol = new ProtocolSI();

            // Ciclo enquanto a flag 'running' for true
            while (running)
            {
                try
                {
                    // --------------------------------------------------------
                    // LEITURA BLOQUEANTE
                    //
                    // networkStream.Read() fica PARADO aqui até chegarem dados.
                    // Quando o servidor enviar algo, os bytes chegam para
                    // recvProtocol.Buffer e o método retorna com o nº de bytes lidos.
                    //
                    // Como esta thread está em background e separada da UI,
                    // o utilizador pode continuar a escrever e clicar botões
                    // enquanto esta espera por dados.
                    // --------------------------------------------------------
                    int bytesRead = networkStream.Read(
                        recvProtocol.Buffer, 0, recvProtocol.Buffer.Length);

                    // 0 bytes = servidor fechou a ligação (sem EOT)
                    if (bytesRead == 0) break;

                    // Nesta fase (Fase I), o servidor só envia pacotes DATA
                    if (recvProtocol.GetCmdType() == ProtocolSICmdType.DATA)
                    {
                        string msg = recvProtocol.GetStringFromData();
                        // Atualizar o chat — deve ser feito na thread da UI
                        AppendMessage(msg);
                    }
                }
                catch
                {
                    // Exceção = ligação interrompida ou running foi para false
                    // e o Disconnect() fechou o stream.
                    // Saímos do ciclo silenciosamente.
                    break;
                }
            }
        }

        // ============================================================
        // ATUALIZAÇÃO DA UI (THREAD-SAFE)
        // ============================================================

        /// <summary>
        /// Adiciona uma linha de texto ao RichTextBox do chat.
        ///
        /// CONCEITO IMPORTANTE: InvokeRequired e thread-safety na UI
        ///
        /// Em Windows Forms, os controlos gráficos (TextBox, RichTextBox,
        /// etc.) SÓ podem ser modificados pela thread que os criou
        /// — a "UI thread" (também chamada main thread).
        ///
        /// A thread de receção (receiveThread) NÃO é a UI thread.
        /// Se ela tentasse fazer richTextBoxChat.AppendText() diretamente,
        /// o programa lançaria uma InvalidOperationException.
        ///
        /// SOLUÇÃO: InvokeRequired + Invoke()
        ///
        ///   InvokeRequired → true se estamos numa thread diferente da UI
        ///   Invoke(action) → executa a action NA thread da UI, de forma segura
        ///
        /// PADRÃO:
        ///   if (InvokeRequired) {
        ///       Invoke(new Action(() => AppendMessage(msg)));
        ///       return;
        ///   }
        ///   // Aqui já estamos na UI thread → podemos modificar controlos
        /// </summary>
        /// <param name="message">Texto a adicionar ao chat</param>
        private void AppendMessage(string message)
        {
            // Verificar se estamos numa thread diferente da UI thread
            if (richTextBoxChat.InvokeRequired)
            {
                // Invocar este mesmo método NA thread da UI.
                // A lambda "() => AppendMessage(message)" é a "action" a executar.
                richTextBoxChat.Invoke(new Action(() => AppendMessage(message)));
                return; // Sair — o Invoke vai chamar AppendMessage de novo na UI thread
            }

            // --------------------------------------------------------
            // Se chegámos aqui, estamos NA UI thread — podemos modificar
            // o controlo gráfico com segurança.
            // --------------------------------------------------------

            // Adicionar a mensagem com uma nova linha no final
            richTextBoxChat.AppendText(message + Environment.NewLine);

            // Fazer scroll automático para a mensagem mais recente
            richTextBoxChat.ScrollToCaret();
        }

        // ============================================================
        // ENVIO DE MENSAGENS
        // ============================================================

        /// <summary>
        /// Lê a mensagem escrita pelo utilizador, mostra-a localmente
        /// e envia-a ao servidor (que a retransmitirá aos outros clientes).
        ///
        /// PORQUÊ o lock(sendLock)?
        ///   Embora normalmente só a UI thread envie mensagens,
        ///   o Disconnect() também escreve no stream (EOT).
        ///   O lock garante que não há escrita simultânea.
        /// </summary>
        private void SendMessage()
        {
            string msg = textBoxMessage.Text.Trim();

            // Não enviar mensagem vazia
            if (string.IsNullOrEmpty(msg)) return;

            // Limpar a caixa de texto e repor o foco nela
            textBoxMessage.Clear();
            textBoxMessage.Focus();

            // --------------------------------------------------------
            // Mostrar a mensagem LOCALMENTE no nosso próprio chat.
            // O servidor retransmite para os OUTROS clientes — não
            // nos envia de volta a nossa própria mensagem.
            // Por isso temos de a mostrar aqui manualmente.
            // --------------------------------------------------------
            AppendMessage("Eu: " + msg);

            try
            {
                // --------------------------------------------------------
                // CRIAR O PACOTE E ENVIAR
                //
                // Make(DATA, msg) → serializa o texto num array de bytes
                //                   com cabeçalho ProtocolSI
                // Write()        → envia os bytes pelo NetworkStream ao servidor
                //
                // lock(sendLock) → garante escrita exclusiva no stream
                // --------------------------------------------------------
                lock (sendLock)
                {
                    byte[] packet = sendProtocol.Make(ProtocolSICmdType.DATA, msg);
                    networkStream.Write(packet, 0, packet.Length);
                }
            }
            catch (Exception ex)
            {
                // Mostrar erro no chat se o envio falhar
                AppendMessage("[Erro ao enviar mensagem: " + ex.Message + "]");
            }
        }

        // ============================================================
        // DESCONEXÃO
        // ============================================================

        /// <summary>
        /// Encerra a ligação ao servidor de forma limpa.
        ///
        /// PROTOCOLO DE DESCONEXÃO:
        ///   1. Enviar pacote EOT (End of Transmission) ao servidor
        ///      → O servidor sabe que este cliente saiu intencionalmente
        ///        e pode notificar os outros ("X saiu do chat")
        ///   2. Fechar o NetworkStream → liberta o canal
        ///   3. Fechar o TcpClient    → liberta o socket TCP
        ///
        /// PORQUÊ a flag isDisconnecting?
        ///   Cenário sem a flag:
        ///     1. Utilizador clica "Desligar" → chama Disconnect() + Close()
        ///     2. Close() dispara o evento FormClosing
        ///     3. FormClosing chama Disconnect() de novo
        ///     4. Segunda chamada tenta escrever num stream já fechado → ERRO
        ///
        ///   Com a flag: a segunda chamada deteta isDisconnecting == true e sai.
        /// </summary>
        private void Disconnect()
        {
            // Proteção contra dupla chamada
            if (isDisconnecting) return;
            isDisconnecting = true;

            // Parar a thread de receção
            running = false;

            try
            {
                // --------------------------------------------------------
                // ENVIAR EOT (End of Transmission)
                //
                // Notifica o servidor que este cliente vai desligar.
                // O servidor usa isto para avisar os outros clientes
                // e remover este handler da lista.
                // --------------------------------------------------------
                lock (sendLock)
                {
                    byte[] eot = sendProtocol.Make(ProtocolSICmdType.EOT);
                    networkStream.Write(eot, 0, eot.Length);
                }

                // Fechar o stream e a ligação TCP
                networkStream.Close();
                tcpClient.Close();
            }
            catch
            {
                // Se o servidor já fechou a ligação, o Write pode falhar.
                // Ignoramos — o objetivo (fechar a ligação) é o mesmo.
            }
        }

        // ============================================================
        // EVENT HANDLERS DA INTERFACE GRÁFICA
        // ============================================================

        /// <summary>
        /// EVENT HANDLER do botão "Enviar".
        /// Chamado quando o utilizador clica no botão.
        /// Delega para o método SendMessage().
        /// </summary>
        private void buttonSend_Click(object sender, EventArgs e)
        {
            SendMessage();
        }

        /// <summary>
        /// EVENT HANDLER de tecla na caixa de mensagem.
        ///
        /// Permite enviar a mensagem premindo Enter, sem precisar
        /// de clicar no botão "Enviar".
        ///
        /// e.Handled = true → impede o Enter de aparecer como texto
        ///                     ou de causar um som de alerta.
        /// </summary>
        private void textBoxMessage_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Return)
            {
                e.Handled = true; // Não processar o Enter como caracter
                SendMessage();
            }
        }

        /// <summary>
        /// EVENT HANDLER do botão "Desligar".
        ///
        /// Desconecta do servidor e fecha a janela.
        /// O Close() vai disparar o evento FormClosing,
        /// que por sua vez chama Disconnect() — mas a flag
        /// isDisconnecting previne a dupla execução.
        /// </summary>
        private void buttonDisconnect_Click(object sender, EventArgs e)
        {
            Disconnect();
            this.Close(); // Fechar a janela (dispara FormClosing)
        }

        /// <summary>
        /// EVENT HANDLER de fecho da janela (X ou Alt+F4).
        ///
        /// FormClosing é disparado SEMPRE que a janela vai fechar,
        /// seja por qual razão for: botão X, Alt+F4, this.Close(),
        /// Application.Exit(), etc.
        ///
        /// Garante que a ligação é sempre fechada corretamente,
        /// mesmo que o utilizador feche a janela pelo X.
        ///
        /// A flag isDisconnecting garante que se o Disconnect() já
        /// foi chamado antes (pelo botão "Desligar"), não é executado
        /// uma segunda vez.
        /// </summary>
        private void FormChat_FormClosing(object sender, FormClosingEventArgs e)
        {
            Disconnect();
        }

        private void FormChat_Load(object sender, EventArgs e)
        {

        }
    }
}
