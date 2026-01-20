namespace E_Commerce.Models
{
    public class Product
    {

        public int Id { get; set; }
        public int CategoryId { get; set; }
        public string? Name { get; set; }
        public string? Model { get; set; }
        public float? Price { get; set; }
        public int? Stock { get; set; }
        public string? Description { get; set; }
        public string? ProductImg { get; set; }
        public bool? IsActive { get; set; }
        public Category Category { get; set; }

    }
}
