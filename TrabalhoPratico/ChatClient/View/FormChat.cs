using EI.SI;
using System;
using System.Net.Sockets;
using System.Threading;
using System.Windows.Forms;

namespace ChatClient
{
    public partial class FormChat : Form
    {
        private readonly TcpClient tcpClient;
        private readonly NetworkStream networkStream;
        private readonly ProtocolSI sendProtocol;
        private readonly string username;
        private Thread receiveThread;
        private volatile bool running = true;
        private bool isDisconnecting = false;
        private readonly object sendLock = new object();

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

            receiveThread = new Thread(ReceiveMessages);
            receiveThread.IsBackground = true;
            receiveThread.Start();
        }

        private void ReceiveMessages()
        {
            ProtocolSI recvProtocol = new ProtocolSI();

            while (running)
            {
                try
                {
                    int bytesRead = networkStream.Read(recvProtocol.Buffer, 0, recvProtocol.Buffer.Length);

                    if (bytesRead == 0) break;

                    if (recvProtocol.GetCmdType() == ProtocolSICmdType.DATA)
                    {
                        AppendMessage(recvProtocol.GetStringFromData());
                    }
                }
                catch
                {
                    break;
                }
            }
        }

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

        private void buttonSend_Click(object sender, EventArgs e)
        {
            SendMessage();
        }

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

        private void FormChat_FormClosing(object sender, FormClosingEventArgs e)
        {
            Disconnect();
        }

        private void FormChat_Load(object sender, EventArgs e)
        {

        }
    }
}
