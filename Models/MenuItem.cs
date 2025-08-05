using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebRestaurant.Models
{
    [Table("menu_items")]
    public class MenuItem
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        public string Description { get; set; }

        [Required]
        public decimal Price { get; set; }

        [Column("image_url")]
        public string ImageUrl { get; set; }

        public string Category { get; set; }

        public bool Available { get; set; } = true;

        [Column("created_at")]
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        // Thêm quan hệ với bảng Review
        public List<Review> Reviews { get; set; } = new List<Review>();
    }
}
