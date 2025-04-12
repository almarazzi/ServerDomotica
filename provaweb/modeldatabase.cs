using Microsoft.EntityFrameworkCore;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace provaweb
{
    public class BloggingContext : DbContext
    {
        public DbSet<Users> Users { get; set; }

        public string DbPath { get; }

        public BloggingContext()
        {
            var programData = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData);
            var database = Path.Combine(programData, "provaweb/database");
            DbPath = System.IO.Path.Join(database, "database.db");
        }

        protected override void OnConfiguring(DbContextOptionsBuilder options)
            => options.UseSqlite($"Data Source={DbPath}");

    }

    public class Users
    {
        [Key]
        public required string UserName { get; set; }
        public required string Password { get; set; }
        public required string Ruolo { get; set; }
        [DefaultValue(true)]
        public required bool StatoAccount { get; set; }



    }


}

