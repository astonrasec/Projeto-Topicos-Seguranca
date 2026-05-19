namespace ChatClient
{
    partial class FormLauncher
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
            this.labelCount = new System.Windows.Forms.Label();
            this.btnAddClient = new System.Windows.Forms.Button();
            this.SuspendLayout();

            // labelTitle
            this.labelTitle.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Bold);
            this.labelTitle.Location = new System.Drawing.Point(20, 20);
            this.labelTitle.Name = "labelTitle";
            this.labelTitle.Size = new System.Drawing.Size(260, 23);
            this.labelTitle.TabIndex = 0;
            this.labelTitle.Text = "Sistema de Chat - Múltiplos Clientes";

            // labelCount
            this.labelCount.Location = new System.Drawing.Point(20, 60);
            this.labelCount.Name = "labelCount";
            this.labelCount.Size = new System.Drawing.Size(260, 23);
            this.labelCount.TabIndex = 1;
            this.labelCount.Text = "Clientes abertos: 0";

            // btnAddClient
            this.btnAddClient.Location = new System.Drawing.Point(20, 100);
            this.btnAddClient.Name = "btnAddClient";
            this.btnAddClient.Size = new System.Drawing.Size(250, 40);
            this.btnAddClient.TabIndex = 2;
            this.btnAddClient.Text = "+ Adicionar Cliente";
            this.btnAddClient.UseVisualStyleBackColor = true;
            this.btnAddClient.Click += new System.EventHandler(this.BtnAddClient_Click);

            // FormLauncher
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(300, 200);
            this.Controls.Add(this.btnAddClient);
            this.Controls.Add(this.labelCount);
            this.Controls.Add(this.labelTitle);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Name = "FormLauncher";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Chat - Launcher";
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        private System.Windows.Forms.Label labelTitle;
        private System.Windows.Forms.Label labelCount;
        private System.Windows.Forms.Button btnAddClient;
    }
}
