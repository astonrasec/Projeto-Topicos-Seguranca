using System;
using System.Windows.Forms;

namespace ChatClient
{
    public partial class FormLauncher : Form
    {
        public FormLauncher()
        {
            InitializeComponent();

            GestorCliente.OnClientCountChanged += UpdateClientCountDisplay;

            this.FormClosing += (s, e) =>
            {
                if (GestorCliente.GetActiveClientCount() > 0)
                {
                    DialogResult result = MessageBox.Show(
                        "Existem clientes abertos. Deseja fechar tudo?",
                        "Confirmar saída",
                        MessageBoxButtons.YesNo,
                        MessageBoxIcon.Question);

                    if (result != DialogResult.Yes)
                        e.Cancel = true;
                }
            };
        }

        private void BtnAddClient_Click(object sender, EventArgs e)
        {
            FormLogin loginForm = new FormLogin();
            loginForm.Show();
        }

        private void UpdateClientCountDisplay(int count)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new Action<int>(UpdateClientCountDisplay), count);
                return;
            }

            if (labelCount != null)
                labelCount.Text = $"Clientes abertos: {count}";
        }
    }
}
