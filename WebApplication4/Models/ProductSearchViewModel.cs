using Microsoft.AspNetCore.Mvc.Rendering;

namespace WebApplication4.Models
{
    public class ProductSearchViewModel
    {
        public List<Product> Products { get; set; }
        public SelectList Categories { get; set; }
        public string SearchString { get; set; }
        public string Category { get; set; }
    }
    public class Product
    {
        public int ProductID { get; set; }
        public int CategoryID { get; set; }
        public string ModelNumber { get; set; }
        public string ModelName { get; set; }
        public string ProductImage { get; set; }
        public decimal UnitCost { get; set; }
        public string Description { get; set; }
        public Category Category { get; set; }
    }
    public class Category
    {
        public int CategoryID { get; set; }
        public string CategoryName { get; set; }
        public ICollection<Product> Products { get; set; }
    }

}
