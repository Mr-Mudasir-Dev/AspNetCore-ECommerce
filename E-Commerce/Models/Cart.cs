namespace E_Commerce.Models
{
    public class Cart
    {
        public int Id { get; set; }

        public int? CustomerId { get; set; }
        public Customer? Customer { get; set; }

        public int? ProductId { get; set; }
        public Product? Product { get; set; }

        public int? Quantity { get; set; }
        public float? Price { get; set; }
        public float? TotalPrice { get; set; }

        public bool? IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
