using Microsoft.EntityFrameworkCore;
using MotoPack_project.Models;

namespace MotoPack_project.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options) { }

        public DbSet<Registar> Registars { get; set; }
        public DbSet<Produto> Produtos { get; set; }
        public DbSet<Suporte> Suportes { get; set; }
        public DbSet<Chat> Chats { get; set; }
        public DbSet<Mensagem> Mensagens { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Produto>()
                .HasOne(p => p.Utilizador)
                .WithMany(u => u.Produtos)
                .HasForeignKey(p => p.UtilizadorId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }

}
