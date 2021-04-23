using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace GestiuneDepozit.Data.ServiceProviderContext
{
    public class SqliteContext : AppDbContext
    {
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite(Configuration.Sqlite_ConnectionString());
            base.OnConfiguring(optionsBuilder);
        }
    }
}
