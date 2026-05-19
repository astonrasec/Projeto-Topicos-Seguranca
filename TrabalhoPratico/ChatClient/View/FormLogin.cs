using EI.SI;
using System;
using System.Net;
using System.Net.Sockets;
using System.Windows.Forms;

namespace ChatClient
{
    public partial class FormLogin : Form
    {
        public FormLogin()
        {
            InitializeComponent();
        }

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
                if (GestorConexao.TentarConectar(username, ip, out TcpClient tcpClient, out NetworkStream stream, out ProtocolSI protocol))
                {
                    FormChat chatForm = new FormChat(tcpClient, stream, protocol, username);

                    chatForm.FormClosed += (s, args) => GestorCliente.UnregisterClient();

                    GestorCliente.RegisterClient();

                    chatForm.Show();
                    this.Hide();
                }
                else
                {
                    MessageBox.Show("O servidor não aceitou a ligação.",
                        "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
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

        private void textBoxUsername_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Return)
            {
                e.Handled = true;
                textBoxIP.Focus();
            }
        }

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
