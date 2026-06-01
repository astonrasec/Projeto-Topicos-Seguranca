using System;
using System.IO;

namespace ChatServer
{
    public class Logger      //Classe para arquivo de log
    {
        private readonly string filePath;      //Caminho do ficheiro onde logs vão ser guardados
        private readonly object lockObj = new object();     //objeto para sincronizar o acesso ao ficheiro

        public Logger(string filePath = "server_log.txt")       //caso nao haja um caminho especificado, o log será guardado em "server_log.txt"
        {
            this.filePath = filePath;

            lock (lockObj)      //garante que apenas uma thread escreve no ficheiro de cada vez
            {
                // Adiciona uma linha indicando o início do log juntamente com a data e hora atual.
                File.AppendAllText(filePath, 
                    $"=== LOG INICIADO - {DateTime.Now:yyyy-MM-dd HH:mm:ss} ===" 
                    + Environment.NewLine);
            }
        }

        public void Log(string level, string message)       //recebe o nível de log (INFO, WARN, ERROR) e a mensagem a ser registrada, e escreve no ficheiro de log
        {
            lock (lockObj)
            {
                string line = $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] [{level}] {message}";
                File.AppendAllText(filePath, line + Environment.NewLine);
            }
        }

        public void Info(string message) => Log("INFO", message);       // Método auxiliar para registar mensagens informativas.
        public void Warn(string message) => Log("WARN", message);       // Método auxiliar para registar avisos.
        public void Error(string message) => Log("ERROR", message);     // Método auxiliar para registar erros.
    }
}
