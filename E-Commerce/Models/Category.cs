namespace E_Commerce.Models
{
    public class Category
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public bool? Status { get; set; }
        public List<Product> Products { get; set; }

    }
}
