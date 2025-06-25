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
<<<<<<< HEAD

=======
>>>>>>> de178ab14944c736a2c455ac24c31151131d2a97
        public DbSet<Suporte> Suportes { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

<<<<<<< HEAD
            modelBuilder.Entity<Produto>()
                .HasOne(p => p.Utilizador)
                .WithMany(u => u.Produtos)
                .HasForeignKey(p => p.UtilizadorId)
                .OnDelete(DeleteBehavior.Cascade);
=======
            // Configurar relação entre Produto e Registar
            modelBuilder.Entity<Produto>()
                .HasOne(p => p.Utilizador)
                .WithMany(u => u.Produtos)
                .HasForeignKey(p => p.UtilizadorId);
>>>>>>> de178ab14944c736a2c455ac24c31151131d2a97
        }
    }
}
