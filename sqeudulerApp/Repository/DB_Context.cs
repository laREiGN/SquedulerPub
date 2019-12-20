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
        { 
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

        public DbSet<Calendar> ScheduleFinal { get; set; }


    }
}
