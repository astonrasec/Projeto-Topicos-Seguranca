using EI.SI;
using System;
using System.Net.Sockets;
using System.Threading;
using System.Windows.Forms;

namespace ChatClient
{
    /// <summary>
    /// Formulário principal do chat.
    /// Mostra o histórico de mensagens, permite enviar texto e recebe
    /// mensagens do servidor em tempo real através de uma thread de background.
    /// </summary>
    public partial class FormChat : Form
    {
        private readonly TcpClient tcpClient;
        private readonly NetworkStream networkStream;

        // Instância de ProtocolSI usada exclusivamente para envio (thread da UI)
        private readonly ProtocolSI sendProtocol;

        private readonly string username;

        // Thread de background que lê continuamente mensagens vindas do servidor
        private Thread receiveThread;

        // Controla o ciclo da thread de receção; volatile garante visibilidade entre threads
        private volatile bool running = true;

        // Impede que o código de desconexão seja executado duas vezes
        // (ex: botão Desligar + evento FormClosing acionados em sequência)
        private bool isDisconnecting = false;

        // Protege escritas simultâneas no NetworkStream por threads diferentes
        private readonly object sendLock = new object();

        /// <summary>
        /// Recebe a ligação já estabelecida pelo FormLogin e inicia
        /// imediatamente a thread de receção de mensagens.
        /// </summary>
        public FormChat(TcpClient client, NetworkStream stream, ProtocolSI protocol, string username)
        {
            InitializeComponent();

            this.tcpClient     = client;
            this.networkStream = stream;
            this.sendProtocol  = protocol;
            this.username      = username;

            this.Text          = "Chat - " + username;
            labelStatus.Text   = "Nome de utilizador: " + username;

            AppendMessage("=== Bem-vindo ao Chat, " + username + "! ===");

            // Lançar thread de receção em background
            receiveThread = new Thread(ReceiveMessages);
            receiveThread.IsBackground = true;
            receiveThread.Start();
        }

        /// <summary>
        /// Corre em background e lê mensagens do servidor continuamente.
        /// Usa uma instância separada de ProtocolSI (recvProtocol) para não
        /// interferir com o sendProtocol que é usado pela thread da UI.
        /// </summary>
        private void ReceiveMessages()
        {
            ProtocolSI recvProtocol = new ProtocolSI();

            while (running)
            {
                try
                {
                    int bytesRead = networkStream.Read(recvProtocol.Buffer, 0, recvProtocol.Buffer.Length);

                    if (bytesRead == 0) break; // Servidor fechou a ligação

                    if (recvProtocol.GetCmdType() == ProtocolSICmdType.DATA)
                    {
                        AppendMessage(recvProtocol.GetStringFromData());
                    }
                }
                catch
                {
                    break; // Ligação interrompida ou stream fechado pelo Disconnect()
                }
            }
        }

        /// <summary>
        /// Adiciona uma linha ao RichTextBox do chat.
        /// Usa InvokeRequired para garantir que a atualização do controlo
        /// é sempre feita na thread da UI, mesmo quando chamado da receiveThread.
        /// </summary>
        private void AppendMessage(string message)
        {
            if (richTextBoxChat.InvokeRequired)
            {
                richTextBoxChat.Invoke(new Action(() => AppendMessage(message)));
                return;
            }

            richTextBoxChat.AppendText(message + Environment.NewLine);
            richTextBoxChat.ScrollToCaret();
        }

        /// <summary>
        /// Lê a mensagem da caixa de texto, mostra-a localmente com o prefixo "Eu:"
        /// e envia-a ao servidor (que a reencaminha aos outros clientes).
        /// O remetente não recebe de volta a sua própria mensagem pelo servidor,
        /// por isso é exibida diretamente aqui.
        /// </summary>
        private void SendMessage()
        {
            string msg = textBoxMessage.Text.Trim();
            if (string.IsNullOrEmpty(msg)) return;

            textBoxMessage.Clear();
            textBoxMessage.Focus();

            AppendMessage("Eu: " + msg);

            try
            {
                lock (sendLock)
                {
                    byte[] packet = sendProtocol.Make(ProtocolSICmdType.DATA, msg);
                    networkStream.Write(packet, 0, packet.Length);
                }
            }
            catch (Exception ex)
            {
                AppendMessage("[Erro ao enviar mensagem: " + ex.Message + "]");
            }
        }

        /// <summary>
        /// Envia EOT ao servidor para sinalizar saída intencional,
        /// depois fecha o stream e a ligação TCP.
        /// A flag isDisconnecting evita dupla execução caso o método
        /// seja chamado pelo botão e pelo FormClosing em sequência.
        /// </summary>
        private void Disconnect()
        {
            if (isDisconnecting) return;
            isDisconnecting = true;
            running = false;

            try
            {
                lock (sendLock)
                {
                    byte[] eot = sendProtocol.Make(ProtocolSICmdType.EOT);
                    networkStream.Write(eot, 0, eot.Length);
                }
                networkStream.Close();
                tcpClient.Close();
            }
            catch { }
        }

        // ===== Eventos da UI =====

        private void buttonSend_Click(object sender, EventArgs e)
        {
            SendMessage();
        }

        /// <summary>
        /// Enter na caixa de mensagem envia sem adicionar nova linha.
        /// </summary>
        private void textBoxMessage_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Return)
            {
                e.Handled = true;
                SendMessage();
            }
        }

        private void buttonDisconnect_Click(object sender, EventArgs e)
        {
            Disconnect();
            this.Close();
        }

        /// <summary>
        /// Garante que a ligação é sempre fechada corretamente,
        /// independentemente de como a janela foi fechada (botão, X, Alt+F4).
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
