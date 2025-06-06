using MotoPack_project.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Win32;
using System.Collections.Generic;

namespace MotoPack_project.Data
{
    public class ApplicantionDbContext : DbContext
    {
        public ApplicantionDbContext(DbContextOptions<ApplicantionDbContext> options) : base(options)
        {
        }

        public DbSet<Registar> Registars { get; set; }


    }
}

