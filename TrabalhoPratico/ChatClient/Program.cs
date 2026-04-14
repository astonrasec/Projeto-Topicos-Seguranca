using System;
using System.Windows.Forms;

namespace ChatClient
{
    /// <summary>
    /// Ponto de entrada da aplicação cliente de chat.
    /// Inicia com o formulário de login.
    /// </summary>
    static class Program
    {
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            // Abrir o formulário de login como janela principal
            Application.Run(new FormLogin());
        }
    }
}
