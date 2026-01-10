using Microsoft.EntityFrameworkCore;

namespace E_Commerce.Models
{
    public class MyDbContext : DbContext
    {
        public MyDbContext(DbContextOptions<MyDbContext>opt) : base (opt) { }

        // Admin Table Add In Database 
        public DbSet <Admin> Admins { get; set; }

    }
}
