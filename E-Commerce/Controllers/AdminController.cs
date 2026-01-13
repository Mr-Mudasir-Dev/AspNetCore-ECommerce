using E_Commerce.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace E_Commerce.Controllers
{
    public class AdminController : Controller
    {
        // Global Variable But Access In AdminController
        private MyDbContext db;
        // wwwroot Folder Access In AdminController 
        private IWebHostEnvironment env;

        // MyDbContext Access in AdminController Constructor & W
        public AdminController(MyDbContext _db, IWebHostEnvironment _env)
        {
            db = _db;
            env = _env;
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

        // Admin Logout 
        public IActionResult Logout()
        {
            if(HttpContext.Session.GetInt32("Admin_id") != null)
            {
                HttpContext.Session.Remove("Admin_id");
                return RedirectToAction("Login");
            }
            return View();
        }

        // Profile Page <-----End Line 141----->
        public IActionResult Profile(int id)
        {
            if (HttpContext.Session.GetInt32("Admin_id") != null)
            {
                var a = db.Admins.Find(id);
                return View(a);
            }
            else
            {
                return RedirectToAction("Login");
            }
            
        }
        [HttpPost]
        public IActionResult Profile(Admin admin, IFormFile Image, int id, string Crunt_Pass, string New_Pass)
        {
            // Fatch Old Data Login Admin
            var olddata = db.Admins.AsNoTracking().FirstOrDefault(a => a.Admin_Id == id);

            // Image Update
            if (Image != null && Image.Length > 0)
            { 
                var filename = Path.GetFileName(Image.FileName);
                var filepath = Path.Combine(env.WebRootPath, "admin/admin_profile_image", filename);
                using(var fs = new FileStream(filepath, FileMode.Create))
                {
                    Image.CopyTo(fs);
                    admin.Admin_Image = filename;
                    TempData["Image"] = "Image Has Been Successfully Updated";
                }
            }else
            {
                if(olddata.Admin_Image != null)
                {
                    admin.Admin_Image = olddata.Admin_Image;
                }
            }

            // Password Update
            if (!string.IsNullOrEmpty(New_Pass))
            {
                if (!string.IsNullOrEmpty(Crunt_Pass) && Crunt_Pass == olddata.Admin_Password)
                {
                    if(New_Pass != olddata.Admin_Password)
                    {
                        admin.Admin_Password = New_Pass;
                        TempData["Success"] = "Your password has been successfully updated";
                    }
                    else
                    {
                        TempData["NewPass=oldpass"] = "New password must be different from current password";
                        return RedirectToAction("Profile");
                    }
                }
                else
                {
                    TempData["Crunt_Pass"] = "Current password is incorrect";
                    return RedirectToAction("Profile");
                }
            }
            else
            {
                admin.Admin_Password = olddata.Admin_Password;
            }

            // Name,Email,Phone Update
            if (admin.Admin_Name != olddata.Admin_Name || admin.Admin_Email != olddata.Admin_Email || admin.Admin_Phone != olddata.Admin_Phone)
            {
                TempData["Profile"] = "Profile Successfully Updated";
            }

            admin.Admin_Id = id;
            db.Admins.Update(admin);
            db.SaveChanges();
            return RedirectToAction("Profile");
        }
        // Profile Page <-----Starte Line 67----->
    }
}
