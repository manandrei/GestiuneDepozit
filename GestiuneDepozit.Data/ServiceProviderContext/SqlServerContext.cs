using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace GestiuneDepozit.Data.ServiceProviderContext
{
    public class SqlServerContext : AppDbContext
    {
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(Configuration.MSSQL_ConnectionString());
            base.OnConfiguring(optionsBuilder);
        }
    }
}
