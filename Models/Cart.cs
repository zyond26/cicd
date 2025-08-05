using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebRestaurant.Models
{
    [Table("cart")]
    public class Cart
    {
        
        [Key]
        public int Id { get; set; }

        [Column("user_id")]
        public int UserId { get; set; }

        [Column("menu_item_id")]
        public int MenuItemId { get; set; }

        public int Quantity { get; set; }

        public string? Note { get; set; }

        [Column("created_at")]
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        [ForeignKey("UserId")]
        public User User { get; set; }

        [ForeignKey("MenuItemId")]
        public MenuItem MenuItem { get; set; }
    }
}
