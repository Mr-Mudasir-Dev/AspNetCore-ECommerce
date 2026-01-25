using E_Commerce.Models;
using E_Commerce.Models.ViewModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;

namespace E_Commerce.Controllers
{
    public class CustomerController : Controller
    {

        // Global Variable But Access In CustomerController
        private MyDbContext db;
        // wwwroot Folder Access In CustomerController
        private IWebHostEnvironment env;

        // MyDbContext Access in CustomerController Constructor
        public CustomerController(MyDbContext _db, IWebHostEnvironment _env)
        {
            db = _db;
            env = _env;
        }


        // Main Page
        public IActionResult Index()
        {
            var vm = new CustomerIndexPage
            {
                Products = db.Products.Where(p => p.IsActive == true).ToList(),
                Categorys = db.Categorys.ToList()
            };

            return View(vm);
        }



        // Register Page
        public IActionResult Register()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Register(Customer cust)
        {

            bool hasError = false;



            // Name Validation
            if(string.IsNullOrEmpty(cust.Name))
            {
                TempData["NameError"] = "Name is required";
                hasError = true;
            }
            else if (!Regex.IsMatch(cust.Name, @"^[A-Za-z]+$"))
            {
                TempData["NameError"] = "Name must contain only letters";
                hasError = true;
            }
            TempData["Name"] = cust.Name;



            // Phone Validation
            if(string.IsNullOrEmpty(cust.Phone))
            {
                TempData["PhoneError"] = "Phone number is required";
                hasError = true;
            }
            else if (!Regex.IsMatch(cust.Phone, @"^[0-9]{11,12}$"))
            {
                TempData["PhoneError"] = "Please enter a valid number (digits only)";
                hasError = true;
            }
            TempData["Phone"] = cust.Phone;



            // Email Validation
            if (string.IsNullOrEmpty(cust.Email))
            {
                TempData["EmailError"] = "Email is required";
                hasError = true;
            }



            // Password Validation
            if (string.IsNullOrEmpty(cust.Password))
            {
                TempData["PasswordError"] = "Email is required";
                hasError = true;
            }



            if (hasError)
            {
                return RedirectToAction("Register");
            }



            db.Customers.Add(cust);
            db.SaveChanges();
            return RedirectToAction("Login");
        }




        // Login Page
        public IActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Login(string user_email, string user_password)
        {
            var log = db.Customers.FirstOrDefault(c => c.Email == user_email);
            if(log != null && log.Password == user_password)
            {
                HttpContext.Session.SetInt32("CustomerId", log.Id);
                return RedirectToAction("Index");
            }
            else
            {
                ViewBag.error = "Incorrect Email/Password";
            }
            return View();
        }



        
        // Logout
        public IActionResult Logout()
        {
            HttpContext.Session.Remove("CustomerId");
            return RedirectToAction("Index");
        }



        // Profile Page
        public IActionResult Profile(int id)
        {
            if(HttpContext.Session.GetInt32("CustomerId") != null)
            {
                var pro = db.Customers.FirstOrDefault(c => c.Id == id);
                return View(pro);
            }
            else
            {
                return RedirectToAction("Login");
            }
        }
        [HttpPost]
        public IActionResult Profile(Customer cust, IFormFile Image, int id)
        {
            var old = db.Customers.AsNoTracking().FirstOrDefault(c => c.Id == id);
            bool hasError = false;
            bool isCorrect = false;


            // Name Validation
            if (string.IsNullOrEmpty(cust.Name))
            {
                TempData["NameError"] = "Name is required";
                hasError = true;
            }
            else if (!Regex.IsMatch(cust.Name, @"^[A-Za-z]+$"))
            {
                TempData["NameError"] = "Name must contain only letters";
                hasError = true;
            }
            if(cust.Name != old.Name)
            {
                isCorrect = true;
            }


            // Phone Validation
            if (string.IsNullOrEmpty(cust.Phone))
            {
                TempData["PhoneError"] = "Phone number is required";
                hasError = true;
            }
            else if (!Regex.IsMatch(cust.Phone, @"^[0-9]{11,12}$"))
            {
                TempData["PhoneError"] = "Please enter a valid number (digits only)";
                hasError = true;
            }
            if (cust.Phone != old.Phone)
            {
                isCorrect = true;
            }



            // Email Validation
            if (string.IsNullOrEmpty(cust.Email))
            {
                TempData["EmailError"] = "Email is required";
                hasError = true;
            }
            if (cust.Email != old.Email)
            {
                isCorrect = true;
            }


            // image
            if (Image != null && Image.Length > 0)
            {
                var filename = Path.GetFileName(Image.FileName);
                var filepath = Path.Combine(env.WebRootPath, "user/User-Image", filename);
                using(var fs = new FileStream(filepath, FileMode.Create))
                {
                    Image.CopyTo(fs);
                    cust.Image = filename;
                    isCorrect = true;
                }
            }
            else
            {
                if(old.Image != null)
                {
                    cust.Image = old.Image;
                    
                }
            }



            if(hasError)
            {
                return RedirectToAction("Profile", new { id = id });
            }


            cust.Password = old.Password;
            db.Customers.Update(cust);
            db.SaveChanges();
            TempData.Clear();
            if(isCorrect)
            {
                TempData["Success"] = cust.Name + " Profile Has been updated";
            }
            return RedirectToAction("Profile", new {id = id});
        }





        // Password Change
        public IActionResult Change_Password(string CurrentPassword, string NewPassword)
        {
            // Customer Id
            var CustomerId = HttpContext.Session.GetInt32("CustomerId");

            // Customer Record where Customer Id
            var old = db.Customers.FirstOrDefault(c => c.Id == CustomerId);

            // Error chack
            bool hasError = false;



            // CurrentPassword fill but NewPassword null = Error
            if (!string.IsNullOrEmpty(CurrentPassword) && string.IsNullOrEmpty(NewPassword))
            {
                TempData["NewPasswordError"] = "If change your password, this field is required";
                hasError = true;
            }

            // CurrentPassword null but NewPassword fill = Error
            if (string.IsNullOrEmpty(CurrentPassword) && !string.IsNullOrEmpty(NewPassword))
            {
                TempData["CurrentPasswordError"] = "Password is required";
                hasError = true;
            }

            // NewPassword fill but Password length under 6 and equal old Password = Error
            if (!string.IsNullOrEmpty(NewPassword))
            {
                if(NewPassword.Length < 6)
                {
                    TempData["NewPasswordError"] = "At least 6 characters Strong Password";
                    hasError = true;
                }
                if (NewPassword.Length > 12)
                {
                    TempData["NewPasswordError"] = "Password cannot exceed 12 characters";
                    hasError = true;
                }

                if (old.Password == NewPassword)
                {
                    TempData["NewPasswordError"] = "New password must be different from current password";
                    hasError = true;
                }
            }

            // CurrentPassword fill but CurrentPassword not equal to old Password = Error
            if (!string.IsNullOrEmpty(CurrentPassword) && old.Password != CurrentPassword)
            {
                TempData["CurrentPasswordError"] = "Current password is incorrect";
                hasError = true;
            }

            // any Error deceted return to back
            if (hasError)
            {
                return RedirectToAction("Profile", new { id = CustomerId });
            }

            // CurrentPassword fill no any Error Finaly SuccessFully Updated Password!
            if (!string.IsNullOrEmpty(CurrentPassword))
            {
                old.Password = NewPassword;
                db.SaveChanges();

                TempData["Success"] = "Password updated successfully";
                return RedirectToAction("Profile", new { id = CustomerId });
            }

            // Both field null = No Error And Back To Page
            return RedirectToAction("Profile", new { id = CustomerId });
        }



        // Product_Detail Page 
        public IActionResult Product_Detail_Page(int id)
        {
            var pro = db.Products.Include(p => p.Category).FirstOrDefault( P => P.Id == id );
            return View(pro);
        }



        // Header Category Based Product Sort Page
        public IActionResult CategoryProductPage(int id)
        {
            var pro = db.Products.Where(p => p.CategoryId == id && p.IsActive == true).ToList();
            ViewBag.cat = db.Categorys.FirstOrDefault(c => c.Id == id);
            return View(pro);
        }

    }
}
