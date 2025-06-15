using MotoPack_project.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Win32;
using System.Collections.Generic;

namespace MotoPack_project.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public DbSet<Registar> Registars { get; set; }

        public DbSet<Produto> Produtos { get; set; }
    }
}
