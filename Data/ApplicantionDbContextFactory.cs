using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System.IO;

namespace MotoPack_project.Data
{
    public class ApplicationDbContextFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
    {
        public ApplicationDbContext CreateDbContext(string[] args)
        { 
            var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();

            var dbPath = Path.Combine(Directory.GetCurrentDirectory(), "Database", "MotoPack.db");
            optionsBuilder.UseSqlite($"Data Source={dbPath}");

            return new ApplicationDbContext(optionsBuilder.Options);
        }
    }
}
