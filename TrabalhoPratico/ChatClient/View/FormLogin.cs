using EI.SI;
using System;
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
            string password = textBoxPassword.Text;
            string ip       = "127.0.0.1"; // IP fixo — campo oculto no designer

            if (string.IsNullOrEmpty(username))
            {
                MessageBox.Show("Por favor introduza um nome de utilizador.",
                    "Campo obrigatório", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                textBoxUsername.Focus();
                return;
            }

            if (string.IsNullOrEmpty(password))
            {
                MessageBox.Show("Por favor introduza a password.",
                    "Campo obrigatório", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                textBoxPassword.Focus();
                return;
            }

            buttonConnect.Enabled = false;

            try
            {
                // TentarConectar devolve a "sessao" própria
                // desta ligação (chave AES/IV, etc.), em vez de usar uma SessaoAtual estática
                bool ligado = GestorConexao.TentarConectar(
                    username, password, ip,
                    out TcpClient tcpClient,
                    out NetworkStream stream,
                    out ProtocolSI protocol,
                    out SessaoAtual sessao);

                if (ligado)
                {
                    FormChat chatForm = new FormChat(tcpClient, stream, protocol, username, sessao);
                    chatForm.FormClosed += (s, args) => GestorCliente.UnregisterClient();
                    GestorCliente.RegisterClient();
                    chatForm.Show();
                    this.Hide();
                }
                else
                {
                    MessageBox.Show("Autenticação falhada. Verifique as suas credenciais.",
                        "Erro de Autenticação", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    buttonConnect.Enabled = true;
                }
            }
            catch (FormatException)
            {
                MessageBox.Show("Endereço IP inválido.",
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
                textBoxPassword.Focus(); // mover foco para password
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

        private void buttonCriarConta_Click(object sender, EventArgs e)
        {
            FormRegisto formRegisto = new FormRegisto();
            formRegisto.Show();
            this.Hide();
            formRegisto.FormClosed += (s, args) => this.Show();
        }

        private void FormLogin_Load(object sender, EventArgs e) { }
    }
}
