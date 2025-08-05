namespace WebRestaurant.Models
{
    public class Payment
    {
        public int Id { get; set; }
        public int OrderId { get; set; }
        public int UserId { get; set; }
        public decimal Amount { get; set; }
        public string PaymentMethod { get; set; } // credit_card, paypal, bank_transfer
        public string Status { get; set; } = "pending";
        public string TransactionId { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}
