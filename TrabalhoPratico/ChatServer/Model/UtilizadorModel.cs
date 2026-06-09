using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ChatServer
{
    
    /// Entidade que representa um utilizador registado na base de dados.
    /// Tabela: Utilizadores
    /// Passwords armazenadas com hash SHA-512 + salt único por utilizador.
    
    [Table("Utilizadores")]
    public class UtilizadorModel
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        [Index(IsUnique = true)]
        public string Username { get; set; }

        [Required]
        [MaxLength(128)]
        public string PasswordHash { get; set; }

        [Required]
        [MaxLength(50)]
        public string Salt { get; set; }

        [Required]
        public DateTime DataRegisto { get; set; }

        public UtilizadorModel() { }

        public UtilizadorModel(string username, string passwordHash, string salt)
        {
            Username     = username;
            PasswordHash = passwordHash;
            Salt         = salt;
            DataRegisto  = DateTime.Now;
        }
    }
}
