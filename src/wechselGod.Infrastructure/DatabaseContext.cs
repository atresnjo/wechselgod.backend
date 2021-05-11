using Microsoft.EntityFrameworkCore;
using wechselGod.Domain;

namespace wechselGod.Infrastructure
{
    public class DatabaseContext : DbContext
    {
        public DbSet<UserAccount> UserAccounts { get; set; }

        public DatabaseContext(DbContextOptions options) : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }
    }
}
