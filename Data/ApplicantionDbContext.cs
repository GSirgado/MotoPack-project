using Microsoft.EntityFrameworkCore;
using MotoPack_project.Models;

namespace MotoPack_project.Data
{
    // Classe de contexto da aplicação, responsável pela ligação à base de dados
    public class ApplicationDbContext : DbContext
    {
        // Construtor que recebe as opções de configuração (como string de conexão)
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options) { }

        // DbSets representam as tabelas da base de dados
        public DbSet<Registar> Registars { get; set; }       
        public DbSet<Produto> Produtos { get; set; }         
        public DbSet<Suporte> Suportes { get; set; }        
        public DbSet<Chat> Chats { get; set; }                
        public DbSet<Mensagem> Mensagens { get; set; }       

        // Configurações adicionais do modelo (relacionamentos, restrições, etc.)
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Define relacionamento entre Produto e Registar (utilizador)
            modelBuilder.Entity<Produto>()
                .HasOne(p => p.Utilizador)                  
                .WithMany(u => u.Produtos)                   
                .HasForeignKey(p => p.UtilizadorId)         
                .OnDelete(DeleteBehavior.Cascade);           
        }
    }
}
