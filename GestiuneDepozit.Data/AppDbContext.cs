using GestiuneDepozit.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace GestiuneDepozit.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext() : base()
        {
            //this.Database.Migrate();
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(Configuration.MSSQL_ConnectionString());
            optionsBuilder.EnableSensitiveDataLogging();
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Status>().HasData(new Status { Id = 1, NumeStatus = "bune" });
        }

        public DbSet<Locatie> Locatii { get; set; }
        public DbSet<Status> Status { get; set; }
        public DbSet<Produs> Produse { get; set; }
        public DbSet<Categorie> Categorii { get; set; }
    }
}
