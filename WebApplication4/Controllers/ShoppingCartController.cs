using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using WebApplication4.Models;
using WebApplication4.Data;
using Microsoft.EntityFrameworkCore;

namespace WebApplication4.Controllers
{
    public class ShoppingCartController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
        private readonly WebApplication4Context _context;
        public ShoppingCartController(WebApplication4Context context)
        {
            _context = context;
        }

        public async Task<IActionResult> Search()
        {
            var categories = await _context.Categories.ToListAsync();

            var productSearchVM = new ProductSearchViewModel
            {
                Categories = new SelectList(await _context.Categories.ToListAsync(), "CategoryID", "CategoryName")
            };
            return View(productSearchVM);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Search(string searchString, string category)
        {
            var products = from p in _context.Products
                           select p;
            if (!string.IsNullOrEmpty(searchString))
            {
                products = products.Where(s => s.ModelName.Contains(searchString));
            }
            if (!string.IsNullOrEmpty(category))
            {
                products = products.Where(x => x.Category.CategoryID.ToString() == category);
            }
            var productSearchVM = new ProductSearchViewModel
            {
                Categories = new SelectList(await _context.Categories.ToListAsync(), "CategoryID", "CategoryName"),
                Products = await products.ToListAsync()
            };
            return View(productSearchVM);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddToCart(int productId, int quantity)
        {
            var product = await _context.Products.FindAsync(productId);
            if (product == null)
            {
                return NotFound();
            }

            // Lấy dữ liệu từ giỏ hàng qua session
            var cart = HttpContext.Session.GetString("ShoppingCart");
            // Khai báo 1 danh sách lưu dữ liệu trong giỏ hàng
            List<ShoppingCartItem> shoppingCartItems;
            if (cart == null)
            {
                shoppingCartItems = new List<ShoppingCartItem>();
            }
            else
            {
                //deserializes chuỗi JSON trong session thành danh sách các ShoppingCartItem
                shoppingCartItems = JsonConvert.DeserializeObject<List<ShoppingCartItem>>(cart);
            }

            // Tìm xem sản phẩm thêm đã có trước trong giỏ hàng hay không
            var shoppingCartItem = shoppingCartItems.FirstOrDefault(x => x.ProductID == productId);
            if (shoppingCartItem == null)
            {
                // chưa có thì thêm mới sản phẩm
                shoppingCartItems.Add(new ShoppingCartItem
                {
                    ProductID = product.ProductID,
                    ProductName = product.ModelName,
                    Price = product.UnitCost,
                    Quantity = quantity,
                });
            }
            else
            {
                //Có rồi thì tăng số lượng lên 1
                shoppingCartItem.Quantity += quantity;
            }
            // Cập nhật lại giỏ hàng bằng cách đổi thành JSON và lưu vào session
            HttpContext.Session.SetString("ShoppingCart", JsonConvert.SerializeObject(shoppingCartItems));
            return RedirectToAction("Search");
        }

        public IActionResult ViewCart()
        {
            var cart = HttpContext.Session.GetString("ShoppingCart");
            List<ShoppingCartItem> shoppingCartItems;
            if (cart == null)
            {
                shoppingCartItems = new List<ShoppingCartItem>();
            }
            else
            {
                shoppingCartItems = JsonConvert.DeserializeObject<List<ShoppingCartItem>>(cart);
            }

            // Lấy thông tin người dùng login
            var currentUser = HttpContext.Session.GetString("CurrentUser");
            // Thêm danh sách giỏ hàng vào viewbag
            ViewBag.ShoppingCartIte = shoppingCartItems;
            // Thêm người login
            ViewBag.CurrentUser = currentUser;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateQuantity(int productId, int quantity)
        {
            // Lấy giỏ hàng
            var cart = HttpContext.Session.GetString("ShoppingCart");
            List<ShoppingCartItem> shoppingCartItems;
            if (cart == null)
            {
                shoppingCartItems = new List<ShoppingCartItem>();
            }
            else
            {
                // chuyển đổi json và lưu vào biến
                shoppingCartItems = JsonConvert.DeserializeObject<List<ShoppingCartItem>>(cart);
            }

            // tìm sản phẩm trùng Id
            var item = shoppingCartItems.FirstOrDefault(i => i.ProductID == productId);
            if (item != null)
            {
                //Chỉnh sửa số lượng
                item.Quantity = quantity;
            }
            HttpContext.Session.SetString("ShoppingCart", JsonConvert.SerializeObject(shoppingCartItems));
            return RedirectToAction("ViewCart");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteItem(int productId, int quantity)
        {
            var cart = HttpContext.Session.GetString("ShoppingCart");
            List<ShoppingCartItem> shoppingCartItems;
            if (cart == null)
            {
                shoppingCartItems = new List<ShoppingCartItem>();
            }
            else
            {
                shoppingCartItems = JsonConvert.DeserializeObject<List<ShoppingCartItem>>(cart);
            }

            var item = shoppingCartItems.FirstOrDefault(i => i.ProductID == productId);
            if (item != null)
            {
                shoppingCartItems.Remove(item);
            }
            HttpContext.Session.SetString("ShoppingCart", JsonConvert.SerializeObject(shoppingCartItems));
            return RedirectToAction("ViewCart");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CheckOut()
        {
            var cart = HttpContext.Session.GetString("ShoppingCart");
            List<ShoppingCartItem> shoppingCartItems;
            if (cart == null)
            {
                return RedirectToAction("ViewCart");
            }
            else
            {
                shoppingCartItems = JsonConvert.DeserializeObject<List<ShoppingCartItem>>(cart);
                var userId = HttpContext.Session.GetString("userId");
                if (userId == null)
                {
                    userId = "1";
                }

                var order = new Orders
                {
                    CustomerID = int.Parse(userId.ToString()),
                    OrderDate = DateTime.Now,
                    ShipDate = DateTime.Now.AddDays(7),
                };
                _context.Add(order);
                _context.SaveChanges();

                foreach (var item in shoppingCartItems)
                {
                    var orderDetail = new OrderDetails
                    {
                        OrderID = order.OrderID,
                        ProductID = item.ProductID,
                        Quantity = item.Quantity,
                        UnitCost = item.Price,
                    };
                    _context.Add(orderDetail);
                    _context.SaveChanges();
                }
                HttpContext.Session.Remove("ShoppingCart");
            }
            return RedirectToAction("ViewCart");
        }
    }
}
