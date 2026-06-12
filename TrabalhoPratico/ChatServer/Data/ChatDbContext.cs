using System.Data.Entity;

namespace ChatServer
{

    public class ChatDbContext : DbContext
    {
        public ChatDbContext() : base("name=ChatDbContext")
        {
            // Cria a BD automaticamente na primeira execução
            Database.SetInitializer(new CreateDatabaseIfNotExists<ChatDbContext>());
        }

        
        public DbSet<UtilizadorModel> Utilizadores { get; set; }
    }
}
