using GestiuneDepozit.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace GestiuneDepozit.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(): base()
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
            modelBuilder.Entity<StatusProdus>().HasData(new StatusProdus { Id = 1, Status = "bune" });
        }

        public DbSet<Locatie> Locatii { get; set; }
        public DbSet<StatusProdus> StatusProdus { get; set; }
        public DbSet<Produs> Produse { get; set; }
    }
}
