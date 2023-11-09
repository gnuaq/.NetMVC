using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WebApplication4.Data;
using WebApplication4.Models;

namespace WebApplication4.Controllers
{
    public class ProductsController : Controller
    {
        private readonly WebApplication4Context _context;
        public ProductsController(WebApplication4Context context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> Search()
        {
            // thực hiện truy vấn các bản ghi từ bảng Categories trong sql và lưu vào biến 
            var categories = await _context.Categories.ToListAsync();

            var productSearchVM = new ProductSearchViewModel
            {
                Categories = new SelectList(await _context.Categories.ToListAsync(), "CategoryID", "CategoryName")
                //Tạo một select list sử dụng trong drop down
                //await _context.Categories.ToListAsync() truy vấn tất cả bản ghi trong Categories
                //"CategoryID" và "CategoryName" là các trường mà select list sẽ sử dụng để đại diện cho các giá trị trong drop down
            };
            return View(productSearchVM);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Search(string searchString, string category)
        //searchString để cho người dùng nhập từ khóa tìm kiếm
        //category cho người dùng chọn danh mục trong ô select
        {
            // Khởi tạo 1 truy vấn từ tất cả các sản phẩm trng csdl
            var products = from p in _context.Products
                           select p;
            if (!string.IsNullOrEmpty(searchString))
            {
                //tìm tất cả các sản phẩm có chứa searchString
                products = products.Where(s => s.ModelName.Contains(searchString));
            }
            if (!string.IsNullOrEmpty(category))
            {
                //Tìm tất cả sản phẩm có cùng category
                products = products.Where(x => x.Category.CategoryID.ToString() == category);
            }
            var productSearchVM = new ProductSearchViewModel
            {
                // Tạo lại 1 danh mục
                Categories = new SelectList(await _context.Categories.ToListAsync(), "CategoryID", "CategoryName"),
                //Lấy các sản phẩm thỏa mãn ô tìm kiếm
                Products = await products.ToListAsync()
            };
            return View(productSearchVM);
        }
    }
}
