using System.ComponentModel.DataAnnotations;

namespace WebApplication4.Models
{
    public class Customers
    {
        [Key] public int CustomerID { get; set; }
        public string FullName { get; set; }
        public string EmailAddress { get; set; }
        public string Password { get; set; }
    }
}