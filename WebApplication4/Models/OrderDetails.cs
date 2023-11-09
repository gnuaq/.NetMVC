using System.ComponentModel.DataAnnotations;

namespace WebApplication4.Models
{
    public class OrderDetails
    {
        [Key] public int OrderID { get; set; }
        public int ProductID { get; set; }
        public int Quantity { get; set; }
        public decimal UnitCost { get; set; }
    }
}
