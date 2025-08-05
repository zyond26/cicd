using Microsoft.EntityFrameworkCore;
using WebRestaurant.Models;

namespace WebRestaurant.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<MenuItem> MenuItems { get; set; }
        public DbSet<Cart> Carts { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderDetail> OrderDetails { get; set; }
        public DbSet<Payment> Payments { get; set; }
        public DbSet<Review> Reviews { get; set; } 

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);


            modelBuilder.Entity<MenuItem>()
       .Property(p => p.Price)
       .HasColumnType("decimal(18,2)");

            modelBuilder.Entity<Order>()
                .Property(o => o.TotalPrice)
                .HasColumnType("decimal(18,2)");

            modelBuilder.Entity<OrderDetail>()
                .Property(o => o.Price)
                .HasColumnType("decimal(18,2)");

            modelBuilder.Entity<Payment>()
                .Property(p => p.Amount)
                .HasColumnType("decimal(18,2)");

            // Cấu hình cho bảng orders
            modelBuilder.Entity<Order>(entity =>
            {
                entity.ToTable("orders");
                entity.Property(e => e.UserId).HasColumnName("user_id");
                entity.Property(e => e.TotalPrice).HasColumnName("total_price");
                entity.Property(e => e.Status).HasColumnName("status");
                entity.Property(e => e.PaymentStatus).HasColumnName("payment_status");
                entity.Property(e => e.CreatedAt).HasColumnName("created_at");
            });

            // Cấu hình cho bảng cart
            modelBuilder.Entity<Cart>(entity =>
            {
                entity.ToTable("cart");
                entity.Property(e => e.UserId).HasColumnName("user_id");
                entity.Property(e => e.MenuItemId).HasColumnName("menu_item_id");
                entity.Property(e => e.CreatedAt).HasColumnName("created_at");
            });

            // Cấu hình cho bảng order_details
            modelBuilder.Entity<OrderDetail>(entity =>
            {
                entity.ToTable("order_details");
                entity.Property(e => e.OrderId).HasColumnName("order_id");
                entity.Property(e => e.MenuItemId).HasColumnName("menu_item_id");
                // Các cột Quantity và Price giữ nguyên tên vì trùng khớp
            });

            // Cấu hình cho bảng payments (nếu cần)
            modelBuilder.Entity<Payment>(entity =>
            {
                entity.ToTable("payments");
            });

            modelBuilder.Entity<Review>(entity =>
            {
                entity.ToTable("reviews");

                entity.HasKey(e => e.Id); // Khóa chính

                entity.Property(e => e.MenuItemId).HasColumnName("menu_item_id").IsRequired();
                entity.Property(e => e.UserName).HasColumnName("user_name").IsRequired().HasMaxLength(100);
                entity.Property(e => e.Rating).HasColumnName("rating").IsRequired();
                entity.Property(e => e.Comment).HasColumnName("comment");
                entity.Property(e => e.CreatedAt).HasColumnName("created_at").HasDefaultValueSql("GETDATE()");

                // Thiết lập quan hệ với MenuItem
                entity.HasOne(e => e.MenuItem)
                    .WithMany(m => m.Reviews)
                    .HasForeignKey(e => e.MenuItemId)
                    .OnDelete(DeleteBehavior.Cascade);
            });
        }
    }
}