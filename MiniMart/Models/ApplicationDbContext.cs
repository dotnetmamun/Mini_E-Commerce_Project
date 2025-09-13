using Microsoft.EntityFrameworkCore;

namespace MiniMart.Models
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }


        public DbSet<Product> Products { get; set; }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            // Add any fluent config if needed

            modelBuilder.Entity<Product>().HasData(
                new Product { Id = 1, Name = "Laptop", Price = 80000, Stock = 10 },
                new Product { Id = 2, Name = "Smartphone", Price = 50000, Stock = 15 },
                new Product { Id = 3, Name = "Headphones", Price = 3000, Stock = 50 },
                new Product { Id = 4, Name = "Keyboard", Price = 1500, Stock = 40 }
            );


            // Seed Customers
            modelBuilder.Entity<Customer>().HasData(
                new Customer { Id = 1, Name = "Alice", Email = "alice@example.com", Phone = "01711111111" },
                new Customer { Id = 2, Name = "Bob", Email = "bob@example.com", Phone = "01722222222" },
                new Customer { Id = 3, Name = "Charlie", Email = "charlie@example.com", Phone = "01733333333" },
                new Customer { Id = 4, Name = "Diana", Email = "diana@example.com", Phone = "01744444444" }
            );
        }
    }
}
