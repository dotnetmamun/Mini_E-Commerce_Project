using System.ComponentModel.DataAnnotations;

namespace MiniMart.Models
{
    public class Product
    {
        public int Id { get; set; }


        [Required]
        public string Name { get; set; }


        [Range(0.01, double.MaxValue, ErrorMessage = "Price must be positive")]
        public decimal Price { get; set; }


        public int Stock { get; set; }
    }

    public class Customer
    {
        public int Id { get; set; }


        [Required]
        public string Name { get; set; }


        [Required, EmailAddress]
        public string Email { get; set; }


        public string Phone { get; set; }

        public virtual ICollection<Order>? Orders { get; set; }
    }

    public class Order
    {
        public int Id { get; set; }
        public int CustomerId { get; set; }
        public Customer Customer { get; set; }
        public DateTime OrderDate { get; set; } = DateTime.UtcNow;
        public decimal TotalAmount { get; set; }
        public ICollection<OrderItem>? OrderItems { get; set; }
    }

    public class OrderItem
    {
        public int Id { get; set; }
        public int OrderId { get; set; }
        public Order Order { get; set; }
        public int ProductId { get; set; }
        public Product Product { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; } // unit price at order time
    }

    public class TopSellingProductDto
    {
        public string Name { get; set; }
        public int TotalQuantity { get; set; }
        public decimal TotalSales { get; set; }
    }

    public class OrderCreateDto
    {
        public int CustomerId { get; set; }
        public List<OrderItemDto> Items { get; set; } = new();
    }

    public class OrderItemDto
    {
        public int ProductId { get; set; }
        public int Quantity { get; set; }
    }

    public class TopSellingReportVm
    {
        public long EfElapsedMs { get; set; }
        public long DapperElapsedMs { get; set; }
        public List<TopSellingProductDto> DapperResults { get; set; }
    }
}
