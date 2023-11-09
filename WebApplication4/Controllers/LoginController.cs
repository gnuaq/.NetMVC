using Microsoft.AspNetCore.Mvc;
using WebApplication4.Data;
using WebApplication4.Models;

namespace WebApplication4.Controllers
{
    public class LoginController : Controller
    {
        private readonly WebApplication4Context _context;
        public LoginController(WebApplication4Context context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            return View(new Customers());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Index(Customers cus)
        {
            if (!string.IsNullOrEmpty(cus.EmailAddress) && (!string.IsNullOrEmpty(cus.Password)))
            {
                //Kiem tra ton tai tai khoan
                var user = _context.Customers.FirstOrDefault(u => u.EmailAddress == cus.EmailAddress && u.Password == cus.Password);
                if (user != null)
                {
                    //Lưu trữ FullName qua từ khóa UserName
                    HttpContext.Session.SetString("UserName", user.FullName);
                    //Chuyển đến trang welcome
                    return RedirectToAction("Welcome");
                }
                else
                {
                    ModelState.AddModelError("", "Invalid email or password !");
                }
            }
            return View(cus);
        }
        public IActionResult Welcome()
        {
            //Lấy thông tin FullName đã lưu ở trên 
            var userName = HttpContext.Session.GetString("UserName");
            if (userName != null)
            {
                ViewBag.UserName = userName;
                return View();
            }
            else
            {
                return RedirectToAction("Index");
            }
        }
    }
}
