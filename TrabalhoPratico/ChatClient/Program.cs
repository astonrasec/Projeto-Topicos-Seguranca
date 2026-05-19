using System;
using System.Windows.Forms;
namespace ChatClient
{
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
