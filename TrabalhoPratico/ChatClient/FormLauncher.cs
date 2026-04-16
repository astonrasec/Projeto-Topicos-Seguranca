using System;
using System.Windows.Forms;

namespace ChatClient
{
    /// <summary>
    /// Formulário launcher - ponto de entrada para múltiplos clientes.
    /// 
    /// RESPONSABILIDADE:
    ///   - Fornecer interface para adicionar novos clientes
    ///   - Manter controle visual do número de clientes ativos
    ///   - Confirmar antes de fechar se há clientes abertos
    /// 
    /// FLUXO:
    ///   1. Abre com contador em 0
    ///   2. Utilizador clica "+ Adicionar Cliente"
    ///   3. FormLogin abre
    ///   4. Quando conecta, ClientManager.RegisterClient() é chamado
    ///   5. FormLauncher recebe evento e atualiza label
    ///   6. Quando fecha FormChat, ClientManager.UnregisterClient() é chamado
    ///   7. FormLauncher recebe evento e decrementa label
    /// </summary>
    public partial class FormLauncher : Form
    {
        private Label labelCount;

        public FormLauncher()
        {
            this.Text = "Chat - Launcher";
            this.StartPosition = FormStartPosition.CenterScreen;
            this.Size = new System.Drawing.Size(300, 200);
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;

            // Título principal
            Label labelTitle = new Label();
            labelTitle.Text = "Sistema de Chat - Múltiplos Clientes";
            labelTitle.Font = new System.Drawing.Font("Arial", 12, System.Drawing.FontStyle.Bold);
            labelTitle.AutoSize = true;
            labelTitle.Location = new System.Drawing.Point(20, 20);
            this.Controls.Add(labelTitle);

            // Label que mostra número de clientes
            labelCount = new Label();
            labelCount.Text = "Clientes abertos: 0";
            labelCount.Location = new System.Drawing.Point(20, 60);
            this.Controls.Add(labelCount);

            // Botão para adicionar novo cliente
            Button btnAddClient = new Button();
            btnAddClient.Text = "+ Adicionar Cliente";
            btnAddClient.Size = new System.Drawing.Size(250, 40);
            btnAddClient.Location = new System.Drawing.Point(20, 100);
            btnAddClient.Click += (s, e) => AddNewClient();
            this.Controls.Add(btnAddClient);

            // Subscrever aos eventos de mudança de contador
            ClientManager.OnClientCountChanged += UpdateClientCountDisplay;

            // Pedir confirmação ao fechar se há clientes abertos
            this.FormClosing += (s, e) =>
            {
                if (ClientManager.GetActiveClientCount() > 0)
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

        /// <summary>
        /// Abre um novo FormLogin para permitir ao utilizador conectar-se.
        /// Chamado quando clica no botão "+ Adicionar Cliente".
        /// </summary>
        private void AddNewClient()
        {
            FormLogin loginForm = new FormLogin();
            loginForm.Show();
        }

        /// <summary>
        /// Callback disparado pelo ClientManager quando o número de clientes muda.
        /// Atualiza o label com o novo número.
        /// 
        /// THREAD-SAFE:
        ///   InvokeRequired verifica se estamos numa thread diferente da UI.
        ///   Se sim, usa Invoke() para voltar à UI thread antes de modificar o label.
        /// </summary>
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
