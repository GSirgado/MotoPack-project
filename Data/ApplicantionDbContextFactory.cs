using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System.IO;

namespace MotoPack_project.Data
{
    public class ApplicationDbContextFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
    {
        public ApplicationDbContext CreateDbContext(string[] args)
<<<<<<< HEAD
        {
            var dbPath = Path.Combine(Directory.GetCurrentDirectory(), "Database", "MotoPack.db");

            var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
=======
        { 
            var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();

            var dbPath = Path.Combine(Directory.GetCurrentDirectory(), "Database", "MotoPack.db");
>>>>>>> de178ab14944c736a2c455ac24c31151131d2a97
            optionsBuilder.UseSqlite($"Data Source={dbPath}");

            return new ApplicationDbContext(optionsBuilder.Options);
        }
    }
}
