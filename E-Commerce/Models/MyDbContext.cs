using Microsoft.EntityFrameworkCore;

namespace E_Commerce.Models
{
    public class MyDbContext : DbContext
    {
        public MyDbContext(DbContextOptions<MyDbContext>opt) : base (opt) { }

        // Admin Table Add In Database 
        public DbSet <Admin> Admins { get; set; }

        // Categorys Table Add In Database 
        public DbSet <Category> Categorys { get; set; }

        // Products Table Add In Database
        public DbSet <Product> Products { get; set; }

        // Custumers Table Add In Database
        public DbSet<Customer> Customers { get; set; }





        // Table Relation Behavior
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // (Product, Category) One Category Many Products 
            modelBuilder.Entity<Product>()
                .HasOne(p => p.Category)
                .WithMany(c => c.Products)
                .HasForeignKey(p => p.CategoryId)
                .OnDelete(DeleteBehavior.Cascade);
        }
        

    }
}
