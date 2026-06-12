using System;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;

namespace ChatServer
{
 
    /// Gera o registo e autenticação de utilizadores na base de dados SQL Server.
    /// Passwords armazenadas com hash SHA-512 + salt aleatório por utilizador.
   
    public class GestorUtilizadores
    {
      
        public bool Registar(string username, string password)
        {
            try
            {
                using (ChatDbContext db = new ChatDbContext())
                {
                    // Verificar se username já existe
                    bool existe = db.Utilizadores.Any(u => u.Username == username);
                    if (existe) return false;

                    // Gerar salt e hash SHA-512
                    string salt = GestorCriptografia.GerarSalt();
                    string hash = GestorCriptografia.HashPassword(password, salt);

                    db.Utilizadores.Add(new UtilizadorModel(username, hash, salt));
                    db.SaveChanges();
                    return true;
                }
            }
            catch (Exception)
            {
                return false;
            }
        }

    
        public bool Autenticar(string username, string password)
        {
            try
            {
                using (ChatDbContext db = new ChatDbContext())
                {
                    UtilizadorModel u = db.Utilizadores
                        .FirstOrDefault(x => x.Username == username);

                    if (u == null) return false;

                    string hashCalculado = GestorCriptografia.HashPassword(password, u.Salt);
                    return hashCalculado.Equals(u.PasswordHash, StringComparison.OrdinalIgnoreCase);
                }
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
