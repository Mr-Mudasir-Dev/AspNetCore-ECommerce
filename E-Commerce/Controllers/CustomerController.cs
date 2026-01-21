using E_Commerce.Models;
using Microsoft.AspNetCore.Mvc;
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


        public IActionResult Index()
        {
            if(HttpContext.Session.Get("CustomerId") != null)
            {
                return View();
            }
            {
                return RedirectToAction("Login");
            }
        }


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
    }
}
