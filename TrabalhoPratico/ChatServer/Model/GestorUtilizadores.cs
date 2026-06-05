using System;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;

namespace ChatServer
{
    /// <summary>
    /// Gere o registo e autenticação de utilizadores na base de dados SQL Server.
    /// Passwords armazenadas com hash SHA-512 + salt aleatório por utilizador.
    /// </summary>
    public class GestorUtilizadores
    {
        /// <summary>
        /// Tenta registar um novo utilizador.
        /// Falha se o username já existir na base de dados.
        /// </summary>
        /// <returns>True se registado com sucesso; False se username já existe.</returns>
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

        /// <summary>
        /// Autentica um utilizador existente.
        /// Falha se o username não existir ou se a password estiver errada.
        /// </summary>
        /// <returns>True se autenticado com sucesso; False caso contrário.</returns>
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
