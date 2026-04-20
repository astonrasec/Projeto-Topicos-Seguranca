using EI.SI;
using System;
using System.Net;
using System.Net.Sockets;
using System.Windows.Forms;

namespace ChatClient
{
    /// <summary>
    /// Formulário de login — primeiro ecrã da aplicação.
    /// Recolhe o nome de utilizador e o IP do servidor,
    /// estabelece a ligação TCP e abre o FormChat se o servidor aceitar.
    /// </summary>
    public partial class FormLogin : Form
    {
        private const int PORT = 10000;

        public FormLogin()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Valida os campos, liga ao servidor via TCP, envia o username
        /// com USER_OPTION_1 e aguarda ACK antes de abrir o FormChat.
        /// O botão é desativado durante a tentativa para evitar cliques duplos.
        /// </summary>
        private void buttonConnect_Click(object sender, EventArgs e)
        {
            string username = textBoxUsername.Text.Trim();
            string ip       = textBoxIP.Text.Trim();

            if (string.IsNullOrEmpty(username))
            {
                MessageBox.Show("Por favor introduza um nome de utilizador.",
                    "Campo obrigatório", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                textBoxUsername.Focus();
                return;
            }

            if (string.IsNullOrEmpty(ip))
            {
                MessageBox.Show("Por favor introduza o endereço IP do servidor.",
                    "Campo obrigatório", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                textBoxIP.Focus();
                return;
            }

            buttonConnect.Enabled = false;

            try
            {
                // Estabelecer ligação TCP ao servidor
                IPEndPoint endpoint = new IPEndPoint(IPAddress.Parse(ip), PORT);
                TcpClient tcpClient = new TcpClient();
                tcpClient.Connect(endpoint);

                NetworkStream stream = tcpClient.GetStream();
                ProtocolSI protocol  = new ProtocolSI();

                // Enviar o username ao servidor — USER_OPTION_1 é o tipo
                // acordado para a mensagem de identificação inicial
                byte[] packet = protocol.Make(ProtocolSICmdType.USER_OPTION_1, username);
                stream.Write(packet, 0, packet.Length);

                // Aguardar confirmação do servidor
                stream.Read(protocol.Buffer, 0, protocol.Buffer.Length);

                if (protocol.GetCmdType() == ProtocolSICmdType.ACK)
                {
                    // Servidor aceitou: passar a ligação já aberta ao FormChat
                     FormChat chatForm = new FormChat(tcpClient, stream, protocol, username);

                     // Quando FormChat fecha, notifica ClientManager para decrementar contador.
                     // Isto permite que FormLauncher fique sincronizado com número real de clientes.
                     chatForm.FormClosed += (s, args) => ClientManager.UnregisterClient();
                     
                     // Registra este cliente como ativo no ClientManager.
                     // ClientManager dispara evento que FormLauncher escuta e atualiza label.
                     ClientManager.RegisterClient();
                     
                     chatForm.Show();
                     this.Hide();
                }
                else
                {
                    MessageBox.Show("O servidor não aceitou a ligação.",
                        "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    stream.Close();
                    tcpClient.Close();
                    buttonConnect.Enabled = true;
                }
            }
            catch (FormatException)
            {
                MessageBox.Show("Endereço IP inválido. Exemplo: 127.0.0.1",
                    "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                buttonConnect.Enabled = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erro ao conectar ao servidor:\n" + ex.Message,
                    "Erro de Ligação", MessageBoxButtons.OK, MessageBoxIcon.Error);
                buttonConnect.Enabled = true;
            }
        }

        /// <summary>
        /// Enter no campo username move o foco para o campo IP.
        /// </summary>
        private void textBoxUsername_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Return)
            {
                e.Handled = true;
                textBoxIP.Focus();
            }
        }

        /// <summary>
        /// Enter no campo IP aciona o botão Conectar diretamente.
        /// </summary>
        private void textBoxIP_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Return)
            {
                e.Handled = true;
                buttonConnect_Click(sender, e);
            }
        }

        private void FormLogin_Load(object sender, EventArgs e)
        {

        }
    }
}
