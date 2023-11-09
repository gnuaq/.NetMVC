using System.ComponentModel.DataAnnotations;

namespace WebApplication4.Models
{
    public class Orders
    {
        [Key] public int OrderID { get; set; }
        public int CustomerID { get; set; }
        public DateTime OrderDate { get; set; }
        public DateTime ShipDate { get; set; }
        public List<OrderDetails> OrderDetails { get; set; }
        public int IDStatus { get; set; }
    }
}
