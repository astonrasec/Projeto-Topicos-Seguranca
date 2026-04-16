using System;
using System.Windows.Forms;

namespace ChatClient
{
    /// <summary>
    /// Ponto de entrada da aplicação cliente de chat.
    /// 
    /// ALTERAÇÃO (Fase I):
    ///   Anteriormente, a aplicação abria um FormLogin único.
    ///   Agora abre FormLauncher que permite múltiplos clientes.
    /// 
    /// NOVO FLUXO:
    ///   1. Application.Run(FormLauncher) - abre o launcher
    ///   2. Launcher mostra interface com botão "+ Adicionar Cliente"
    ///   3. Cada clique abre um novo FormLogin
    ///   4. FormLogin conecta ao servidor e abre FormChat
    ///   5. Múltiplas instâncias de FormChat rodam simultaneamente
    /// </summary>
    static class Program
    {
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            
            FormLauncher launcher = new FormLauncher();
            Application.Run(launcher);
        }
    }
}
