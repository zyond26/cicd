using System.ComponentModel.DataAnnotations.Schema;

namespace WebRestaurant.Models
{
    public class Order
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public decimal TotalPrice { get; set; }
        public string Status { get; set; } = "pending";
        public string PaymentStatus { get; set; } = "pending";
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public List<OrderDetail> OrderDetails { get; set; }

        [ForeignKey("UserId")]
        public User User { get; set; }
    }
}
