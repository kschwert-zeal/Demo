using Demo.Models;
using Microsoft.EntityFrameworkCore;

namespace Demo.Database
{
    public class DBContext : DbContext
    {
        public DBContext(DbContextOptions<DBContext> options) : base(options) { }

        public DbSet<Users> Users { get; set; }

    }
}
