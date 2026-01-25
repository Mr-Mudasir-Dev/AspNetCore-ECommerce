using E_Commerce.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace E_Commerce.ViewComponents
{
    public class CategoryMenuViewComponent : ViewComponent
    {
        private MyDbContext db;
        public CategoryMenuViewComponent(MyDbContext _db)
        {
            db = _db;
        }

        public IViewComponentResult Invoke()
        {
            var cat = db.Categorys.Where(c => c.Status == true).Include(c => c.Products).ToList();
            return View(cat);
        }
    }
}
