using E_Commerce.Migrations;
using E_Commerce.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;

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
            if (HttpContext.Session.GetInt32("Admin_id") != null)
            {
                HttpContext.Session.Remove("Admin_id");
                return RedirectToAction("Login");
            }
            return View();
        }

        // Profile Page    <-------- End Line 141 -------->
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
                using (var fs = new FileStream(filepath, FileMode.Create))
                {
                    Image.CopyTo(fs);
                    admin.Admin_Image = filename;
                    TempData["Image"] = "Image Has Been Successfully Updated";
                }
            } else
            {
                if (olddata.Admin_Image != null)
                {
                    admin.Admin_Image = olddata.Admin_Image;
                }
            }

            // Password Update
            if (!string.IsNullOrEmpty(New_Pass))
            {
                if (!string.IsNullOrEmpty(Crunt_Pass) && Crunt_Pass == olddata.Admin_Password)
                {
                    if (New_Pass != olddata.Admin_Password)
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
        // Profile Page     <-------- Start Line 67 -------->






        // Category   <-------- End Line 253 -------->


        //Category All Record Show Page
        public IActionResult Category_All_Show()
        {
            var category = db.Categorys.ToList();
            return View(category);
        }


        //Category Create Page
        public IActionResult Create_Category()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Create_Category(Category cat, string CategoryName)
        {
            if (string.IsNullOrEmpty(CategoryName))
            {
                TempData["catnull"] = "Category name is required";
                return RedirectToAction("Create_Category");
            }

            if (!System.Text.RegularExpressions.Regex.IsMatch(CategoryName, @"^[A-Za-z]+$"))
            {
                TempData["caterror"] = "Category name must contain only letters.";
                return RedirectToAction("Create_Category");
            }
            else
            {
                cat.Name = CategoryName;
                db.Categorys.Add(cat);
                db.SaveChanges();
                TempData["Create-Success"] = "Your Category has been added successfully";
                return RedirectToAction("Category_All_Show");
            }
        }


        //Category Edit Page
        public IActionResult Category_Edit(int id)
        {
            var edit = db.Categorys.FirstOrDefault(c => c.Id == id);
            return View(edit);
        }
        [HttpPost]
        public IActionResult Category_Edit(Category cat, string CategoryName, int id)
        {
            var old = db.Categorys.AsNoTracking().FirstOrDefault(c => c.Id == id);


            if (string.IsNullOrEmpty(CategoryName))
            {
                TempData["catnull"] = "Category name is required";
                return RedirectToAction("Category_Edit");
            }

            if (CategoryName == old.Name)
            {
                TempData["cat-old"] = "Your Category Name As Old If You No Need To Edit Go Back";
                return RedirectToAction("Category_Edit");
            }

            if (!Regex.IsMatch(CategoryName, @"^[A-Za-z]+$"))
            {
                TempData["caterror"] = "Category name must contain only letters.";
                return RedirectToAction("Category_Edit");
            }
            else
            {
                cat.Name = CategoryName;
                db.Categorys.Update(cat);
                db.SaveChanges();
                TempData["Edit-Success"] = "Your Category " + cat.Name + " has been Edit successfully";
                return RedirectToAction("Category_All_Show");
            }

        }


        //Category Delete Permission Page
        public IActionResult Category_Delete_Permission(int id)
        {
            var idrecord = db.Categorys.FirstOrDefault(c => c.Id == id);
            return View(idrecord);
        }


        //Category Delete Recode Where Id
        public IActionResult Category_Delete(int id)
        {
            var delete = db.Categorys.Find(id);
            db.Categorys.Remove(delete);
            db.SaveChanges();
            TempData["Success"] = "Category " + delete.Name + " Has Been Deleted Successfully";
            return RedirectToAction("Category_All_Show");
        }

        // Category   <--------- Start Line 147 --------->




        // Product   <--------- End Line 147 --------->
        public IActionResult Product_Index()
        {
            if (HttpContext.Session.GetInt32("Admin_id") != null)
            {
                var pro = db.Products.ToList();
                return View(pro);
            }
            else
            {
                return RedirectToAction("Login");
            }
            
        }

        public IActionResult Product_Create()
        {
            if (HttpContext.Session.GetInt32("Admin_id") != null)
            {
                var cat = db.Categorys.ToList();
                ViewBag.cat = cat;
                return View();
            }
            else
            {
                return RedirectToAction("Login");
            }
        }
        [HttpPost]
        public IActionResult Product_Create(Product pro, IFormFile Image)
        {

            bool hasError = false;

            // Name Validation
            if (string.IsNullOrWhiteSpace(pro.Name))
            {
                TempData["Name"] = "Name is required";
                hasError = true;
            }else if(!Regex.IsMatch(pro.Name, @"^[A-Za-z]+$"))
            {
                TempData["Name-err"] = "Product name must contain only letters";
                hasError = true;
            }else if(pro.Name.Length <= 4)
            {
                TempData["Name-length"] = "Please enter a valid product name";
                hasError = true;
            }
            TempData["NameValue"] = pro.Name;


            // CategoryId Validation
            if (pro.CategoryId == null || pro.CategoryId == 0)
            {
                TempData["CategoryId"] = "Category is required";
                hasError = true;
            }


            // Model Validation
            if (string.IsNullOrWhiteSpace(pro.Model))
            {
                TempData["Model"] = "Model is required";
                hasError = true;
            }else if(!Regex.IsMatch(pro.Model, @"^[A-Za-z0-9]+$"))
            {
                TempData["Model-err"] = "Model can contain only letters and numbers";
                hasError = true;
            }
            TempData["ModelValue"] = pro.Model;


            // Price Validation
            if (pro.Price == null)
            {
                TempData["Price"] = "Price is required";
                hasError = true;
            }
            TempData["PriceValue"] = pro.Price.ToString();


            // Stock Validation
            if (pro.Stock == null)
            {
                TempData["Stock"] = "Stock is required";
                hasError = true;
            }
            TempData["StockValue"] = pro.Stock?.ToString();


            // Description Validation
            if (string.IsNullOrWhiteSpace(pro.Description))
            {
                TempData["Description"] = "Description is required";
                hasError = true;
            }
            TempData["DescriptionValue"] = pro.Description;



            if (hasError)
            {
                TempData["haserror"] = "Please fill all required fields";
                return RedirectToAction("Product_Create");
            }

            TempData.Clear();
            db.Products.Add(pro);
            db.SaveChanges();
            TempData["Success-Create"] = "Product Create Has Been Successfully!";
            return RedirectToAction("Product_Index");
        }
        // Product   <--------- Start Line 258 --------->

        public IActionResult Product_Delete_Premission(int id)
        {
            var pro = db.Products.FirstOrDefault(p => p.Id == id);
            return View(pro);
        }
        public IActionResult Product_Delete(int id)
        {
            var delete = db.Products.Find(id);
            db.Products.Remove(delete);
            db.SaveChanges();
            TempData["Pruduct-Delete"] = "Product has been Drop Successfully!";
            return RedirectToAction("Product_Index");
        }


    }
}
