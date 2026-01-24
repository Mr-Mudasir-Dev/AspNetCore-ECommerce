namespace E_Commerce.Models.ViewModel
{
    public class CustomerIndexPage
    {
        public List<Product> Products { get; set; } = new List<Product>();
        public List<Category> Categorys { get; set; } = new List<Category>();
    }
}
