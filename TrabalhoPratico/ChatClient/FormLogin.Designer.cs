namespace ChatClient
{
    partial class FormLogin
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
                components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.labelTitle = new System.Windows.Forms.Label();
            this.labelUsername = new System.Windows.Forms.Label();
            this.textBoxUsername = new System.Windows.Forms.TextBox();
            this.labelIP = new System.Windows.Forms.Label();
            this.textBoxIP = new System.Windows.Forms.TextBox();
            this.buttonConnect = new System.Windows.Forms.Button();
            this.SuspendLayout();

            // labelTitle
            this.labelTitle.AutoSize = false;
            this.labelTitle.Font = new System.Drawing.Font("Segoe UI", 14F, System.Drawing.FontStyle.Bold);
            this.labelTitle.Location = new System.Drawing.Point(0, 20);
            this.labelTitle.Name = "labelTitle";
            this.labelTitle.Size = new System.Drawing.Size(300, 30);
            this.labelTitle.TabIndex = 0;
            this.labelTitle.Text = "Chat - Ligação ao Servidor";
            this.labelTitle.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;

            // labelUsername
            this.labelUsername.AutoSize = true;
            this.labelUsername.Location = new System.Drawing.Point(30, 70);
            this.labelUsername.Name = "labelUsername";
            this.labelUsername.TabIndex = 1;
            this.labelUsername.Text = "Nome de Utilizador:";

            // textBoxUsername
            this.textBoxUsername.Location = new System.Drawing.Point(30, 90);
            this.textBoxUsername.Name = "textBoxUsername";
            this.textBoxUsername.Size = new System.Drawing.Size(240, 20);
            this.textBoxUsername.TabIndex = 2;
            this.textBoxUsername.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.textBoxUsername_KeyPress);

            // labelIP
            this.labelIP.AutoSize = true;
            this.labelIP.Location = new System.Drawing.Point(30, 125);
            this.labelIP.Name = "labelIP";
            this.labelIP.TabIndex = 3;
            this.labelIP.Text = "IP do Servidor:";

            // textBoxIP
            this.textBoxIP.Location = new System.Drawing.Point(30, 145);
            this.textBoxIP.Name = "textBoxIP";
            this.textBoxIP.Size = new System.Drawing.Size(240, 20);
            this.textBoxIP.TabIndex = 4;
            this.textBoxIP.Text = "127.0.0.1";
            this.textBoxIP.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.textBoxIP_KeyPress);

            // buttonConnect
            this.buttonConnect.Location = new System.Drawing.Point(90, 185);
            this.buttonConnect.Name = "buttonConnect";
            this.buttonConnect.Size = new System.Drawing.Size(120, 35);
            this.buttonConnect.TabIndex = 5;
            this.buttonConnect.Text = "Conectar";
            this.buttonConnect.UseVisualStyleBackColor = true;
            this.buttonConnect.Click += new System.EventHandler(this.buttonConnect_Click);

            // FormLogin
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(300, 245);
            this.Controls.Add(this.buttonConnect);
            this.Controls.Add(this.textBoxIP);
            this.Controls.Add(this.labelIP);
            this.Controls.Add(this.textBoxUsername);
            this.Controls.Add(this.labelUsername);
            this.Controls.Add(this.labelTitle);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Name = "FormLogin";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Chat - Ligação";
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        private System.Windows.Forms.Label labelTitle;
        private System.Windows.Forms.Label labelUsername;
        private System.Windows.Forms.TextBox textBoxUsername;
        private System.Windows.Forms.Label labelIP;
        private System.Windows.Forms.TextBox textBoxIP;
        private System.Windows.Forms.Button buttonConnect;
    }
}
