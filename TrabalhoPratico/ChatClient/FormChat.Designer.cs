namespace ChatClient
{
    partial class FormChat
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
            this.labelStatus = new System.Windows.Forms.Label();
            this.richTextBoxChat = new System.Windows.Forms.RichTextBox();
            this.labelMessage = new System.Windows.Forms.Label();
            this.textBoxMessage = new System.Windows.Forms.TextBox();
            this.buttonSend = new System.Windows.Forms.Button();
            this.buttonDisconnect = new System.Windows.Forms.Button();
            this.SuspendLayout();

            // labelStatus - mostra o utilizador conectado
            this.labelStatus.AutoSize = true;
            this.labelStatus.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.labelStatus.Location = new System.Drawing.Point(10, 10);
            this.labelStatus.Name = "labelStatus";
            this.labelStatus.TabIndex = 0;
            this.labelStatus.Text = "Conectado como: ";

            // richTextBoxChat - área principal de mensagens (só leitura)
            this.richTextBoxChat.BackColor = System.Drawing.Color.White;
            this.richTextBoxChat.Location = new System.Drawing.Point(10, 35);
            this.richTextBoxChat.Name = "richTextBoxChat";
            this.richTextBoxChat.ReadOnly = true;
            this.richTextBoxChat.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.Vertical;
            this.richTextBoxChat.Size = new System.Drawing.Size(460, 310);
            this.richTextBoxChat.TabIndex = 1;
            this.richTextBoxChat.Text = "";

            // labelMessage
            this.labelMessage.AutoSize = true;
            this.labelMessage.Location = new System.Drawing.Point(10, 358);
            this.labelMessage.Name = "labelMessage";
            this.labelMessage.TabIndex = 2;
            this.labelMessage.Text = "Mensagem:";

            // textBoxMessage - caixa para escrever a mensagem
            this.textBoxMessage.Location = new System.Drawing.Point(10, 375);
            this.textBoxMessage.Name = "textBoxMessage";
            this.textBoxMessage.Size = new System.Drawing.Size(360, 20);
            this.textBoxMessage.TabIndex = 3;
            this.textBoxMessage.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.textBoxMessage_KeyPress);

            // buttonSend - botão enviar
            this.buttonSend.Location = new System.Drawing.Point(380, 372);
            this.buttonSend.Name = "buttonSend";
            this.buttonSend.Size = new System.Drawing.Size(90, 26);
            this.buttonSend.TabIndex = 4;
            this.buttonSend.Text = "Enviar";
            this.buttonSend.UseVisualStyleBackColor = true;
            this.buttonSend.Click += new System.EventHandler(this.buttonSend_Click);

            // buttonDisconnect - botão desligar
            this.buttonDisconnect.Location = new System.Drawing.Point(380, 405);
            this.buttonDisconnect.Name = "buttonDisconnect";
            this.buttonDisconnect.Size = new System.Drawing.Size(90, 26);
            this.buttonDisconnect.TabIndex = 5;
            this.buttonDisconnect.Text = "Desligar";
            this.buttonDisconnect.UseVisualStyleBackColor = true;
            this.buttonDisconnect.Click += new System.EventHandler(this.buttonDisconnect_Click);

            // FormChat
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(484, 441);
            this.Controls.Add(this.buttonDisconnect);
            this.Controls.Add(this.buttonSend);
            this.Controls.Add(this.textBoxMessage);
            this.Controls.Add(this.labelMessage);
            this.Controls.Add(this.richTextBoxChat);
            this.Controls.Add(this.labelStatus);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Name = "FormChat";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Chat";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FormChat_FormClosing);
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        private System.Windows.Forms.Label labelStatus;
        private System.Windows.Forms.RichTextBox richTextBoxChat;
        private System.Windows.Forms.Label labelMessage;
        private System.Windows.Forms.TextBox textBoxMessage;
        private System.Windows.Forms.Button buttonSend;
        private System.Windows.Forms.Button buttonDisconnect;
    }
}
