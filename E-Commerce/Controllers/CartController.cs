using E_Commerce.Migrations;
using E_Commerce.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;



namespace E_Commerce.Controllers
{
    public class CartController : Controller
    {
        // Global Variable But Access In CartController
        private MyDbContext db;

        // MyDbContext Access in CartController Constructor
        public CartController(MyDbContext _db)
        {
            db = _db;
        }



        // Main Cart Page
        public IActionResult Index()
        { 
            if(HttpContext.Session.GetInt32("CustomerId") != null)
            {
                var cId = HttpContext.Session.GetInt32("CustomerId");
                var cart = db.Carts.Where(c => c.CustomerId == cId).Include(c => c.Customer).Include(c => c.Product).ThenInclude(p => p.Category).ToList();
                return View(cart);
            }
            else
            {
                return RedirectToAction("Login","Customer");
            }
        }

        // Add To Cart Logic
        public IActionResult AddToCart(int pId)
        {
            if(HttpContext.Session.GetInt32("CustomerId") != null)
            {
                var product = db.Products.FirstOrDefault(p => p.Id == pId);
                var CustomerId = HttpContext.Session.GetInt32("CustomerId");

                var existingcart = db.Carts.FirstOrDefault(c => c.ProductId == pId && c.CustomerId == CustomerId);

                if (existingcart != null)
                {
                    existingcart.Quantity += 1;
                    existingcart.TotalPrice = existingcart.Quantity * existingcart.Price;
                }
                else
                {
                    var cart = new Cart
                    {
                        ProductId = pId,
                        CustomerId = CustomerId,
                        Quantity = 1,
                        Price = product.Price ?? 0,
                        TotalPrice = product.Price ?? 0,
                        IsActive = false,
                        CreatedAt = DateTime.Now

                    };

                    db.Carts.Add(cart);
                }


                db.SaveChanges();
                TempData["Success"] = product.Name + " product Add To Cart";
                return RedirectToAction("Index");
            }
            else
            {
                return RedirectToAction("Login","Customer");
            }
           
        }


        public IActionResult UpdateQty(int cartId, string type)
        {
            var cart = db.Carts.Find(cartId);

            if(type == "inc")
            {
                cart.Quantity++;
            }else if(type == "dec")
            {
                cart.Quantity--;
            }

            if(type == "dec" && cart.Quantity == 0)
            {
                return RedirectToAction("DeleteToCart", new {id = cart.Id});
            }

            cart.TotalPrice = cart.Price * cart.Quantity;
            db.SaveChanges();
            return RedirectToAction("Index");

        }




        // Delete Cart Record Logic
        public IActionResult DeleteToCart(int id)
        {
            var delete = db.Carts.Find(id);
            if (delete != null)
            {
                db.Carts.Remove(delete);
                db.SaveChanges();
            }

            TempData["Success"] = "Item removed from cart";
            return RedirectToAction("Index");
        }
    }
}
