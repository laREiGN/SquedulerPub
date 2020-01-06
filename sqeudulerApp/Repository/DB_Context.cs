using Microsoft.EntityFrameworkCore;
using sqeudulerApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace sqeudulerApp.Repository
{
    public class DB_Context : DbContext
    {
        public DB_Context(DbContextOptions<DbContext> options) : base(options)
        {  //var optionsBuilder = new DbContextOptionsBuilder<DB_Context>();
            //optionsBuilder.UseSqlServer("Server=tcp:squeduler.database.windows.net,1433;Initial Catalog=squeduler;Persist Security Info=False;User ID=user;Password=squeduler#123;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;");

        }
        public DB_Context()
        {

        }

        public DB_Context(DbContextOptions options) : base(options)
        {
        }
        public DbSet<User> User { get; set; }

        public DbSet<Teams> Teams { get; set; }


        public DbSet<UserTeam> UserTeam { get; set; }

        public DbSet<Requests> Requests { get; set; }
    }
}
