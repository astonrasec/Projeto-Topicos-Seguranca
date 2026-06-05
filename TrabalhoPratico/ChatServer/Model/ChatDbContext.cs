using System.Data.Entity;

namespace ChatServer
{
    /// <summary>
    /// Contexto Entity Framework para a base de dados do Chat.
    /// Liga ao SQL Server LocalDB definido no App.config (TrabalhoPratico).
    /// A base de dados e a tabela são criadas automaticamente se não existirem.
    /// </summary>
    public class ChatDbContext : DbContext
    {
        public ChatDbContext() : base("name=ChatDbContext")
        {
            // Cria a BD automaticamente na primeira execução
            Database.SetInitializer(new CreateDatabaseIfNotExists<ChatDbContext>());
        }

        /// <summary>
        /// Tabela de utilizadores registados.
        /// </summary>
        public DbSet<UtilizadorModel> Utilizadores { get; set; }
    }
}
