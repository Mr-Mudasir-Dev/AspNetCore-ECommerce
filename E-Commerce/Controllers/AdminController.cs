using E_Commerce.Models;
using Microsoft.AspNetCore.Mvc;

namespace E_Commerce.Controllers
{
    public class AdminController : Controller
    {
        // Global Variable But Access In AdminController
        private MyDbContext db;

        // MyDbContext Access in AdminController Constructor
        public AdminController(MyDbContext _db)
        {
            db = _db;
        }


        // Main Dashboard page
        public IActionResult Index()
        {
            if (HttpContext.Session.GetInt32("Admin_id") != null)
            {
                return View();
            }
            else
            {
                return RedirectToAction("Login");
            }
        }

        // Login Page 
        public IActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Login(string Email, string Password)
        {
            var auth = db.Admins.FirstOrDefault(a => a.Admin_Email == Email);
            if (auth != null && auth.Admin_Password == Password)
            {
                HttpContext.Session.SetInt32("Admin_id", auth.Admin_Id);
                return RedirectToAction("Index");
            }
            else
            {
                ViewBag.error = "Invalid Email/Password";
            }
            return View();
        }

    }
}
