using System.ComponentModel.DataAnnotations;

namespace WebApplication4.Models
{
    public class ShoppingCartItem
    {
        [Key] public int ProductID { get; set; }
        public string ProductName { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }
        public decimal Total => Price * Quantity;
    }
}
