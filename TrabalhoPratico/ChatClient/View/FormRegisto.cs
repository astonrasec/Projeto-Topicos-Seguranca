using EI.SI;
using System;
using System.Net.Sockets;
using System.Windows.Forms;

namespace ChatClient
{
  
    public partial class FormRegisto : Form
    {
        public FormRegisto()
        {
            InitializeComponent();
        }

        
        private void buttonRegistar_Click(object sender, EventArgs e)
        {
            string username = textBoxUsername.Text.Trim();
            string password = textBoxPassword.Text;
            string confirm  = textBoxConfirm.Text;
            string ip       = "127.0.0.1";

            // Validação dos campos
            if (string.IsNullOrEmpty(username))
            {
                MessageBox.Show("Por favor introduza um nome de utilizador.",
                    "Campo obrigatório", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                textBoxUsername.Focus();
                return;
            }

            if (string.IsNullOrEmpty(password))
            {
                MessageBox.Show("Por favor introduza uma password.",
                    "Campo obrigatório", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                textBoxPassword.Focus();
                return;
            }

            if (password.Length < 6)
            {
                MessageBox.Show("A password deve ter no mínimo 6 caracteres.",
                    "Password fraca", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                textBoxPassword.Focus();
                return;
            }

            if (password != confirm)
            {
                MessageBox.Show("As passwords não coincidem.",
                    "Erro de confirmação", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                textBoxConfirm.Clear();
                textBoxConfirm.Focus();
                return;
            }

            buttonRegistar.Enabled = false;

            try
            {
                bool registado = GestorConexao.TentarRegistar(
                    username, password, ip,
                    out TcpClient tcpClient,
                    out NetworkStream stream,
                    out ProtocolSI protocol);

                if (registado)
                {
                    MessageBox.Show("Conta criada com sucesso! A entrar no chat...",
                        "Registo", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    FormChat chatForm = new FormChat(tcpClient, stream, protocol, username);
                    chatForm.FormClosed += (s, args) => GestorCliente.UnregisterClient();
                    GestorCliente.RegisterClient();
                    chatForm.Show();
                    this.Close();
                }
                else
                {
                    MessageBox.Show("O nome de utilizador já existe. Escolha outro.",
                        "Registo falhado", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    buttonRegistar.Enabled = true;
                    textBoxUsername.Focus();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erro ao ligar ao servidor:\n" + ex.Message,
                    "Erro de Ligação", MessageBoxButtons.OK, MessageBoxIcon.Error);
                buttonRegistar.Enabled = true;
            }
        }

        private void buttonCancelar_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void textBoxUsername_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Return) { e.Handled = true; textBoxPassword.Focus(); }
        }

        private void textBoxPassword_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Return) { e.Handled = true; textBoxConfirm.Focus(); }
        }

        private void textBoxConfirm_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Return) { e.Handled = true; buttonRegistar_Click(sender, e); }
        }

        private void FormRegisto_Load(object sender, EventArgs e)
        {

        }
    }
}
