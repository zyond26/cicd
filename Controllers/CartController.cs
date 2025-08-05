using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System.Threading.Tasks;
using WebRestaurant.Data;
using WebRestaurant.Models;

namespace WebRestaurant.Controllers
{
    [Authorize]
    public class CartController : Controller
    {
        private readonly AppDbContext _context;

        public CartController(AppDbContext context)
        {
            _context = context;
        }

        // Hiển thị giỏ hàng của người dùng
        public async Task<IActionResult> Index()
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            var cartItems = await _context.Carts
                                          .Include(c => c.MenuItem)
                                          .Where(c => c.UserId == userId)
                                          .ToListAsync();
            return View(cartItems);
        }

        // Thêm sản phẩm vào giỏ hàng và chuyển hướng về returnUrl nếu có
        [HttpPost]
        public async Task<IActionResult> AddToCart(int menuItemId, int quantity, string returnUrl)
        {
            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Login", "Auth");
            }

            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null)
            {
                return RedirectToAction("Login", "Auth");
            }

            int userId = int.Parse(userIdClaim.Value);

            var existingItem = await _context.Carts
                                             .FirstOrDefaultAsync(c => c.UserId == userId && c.MenuItemId == menuItemId);
            if (existingItem != null)
            {
                existingItem.Quantity += quantity;
            }
            else
            {
                var cartItem = new Cart
                {
                    UserId = userId,
                    MenuItemId = menuItemId,
                    Quantity = quantity
                };
                _context.Carts.Add(cartItem);
            }
            await _context.SaveChangesAsync();

            // Thêm thông báo vào TempData để hiển thị trong giao diện
            TempData["CartSuccessMessage"] = "Món ăn đã được thêm vào giỏ hàng!";

            if (!string.IsNullOrEmpty(returnUrl))
            {
                return Redirect(returnUrl);
            }
            return RedirectToAction("Index", "Menu");
        }


        // Cập nhật số lượng của một món trong giỏ hàng
        [HttpPost]
        public async Task<IActionResult> UpdateCart(int id, int quantity)
        {
            var cartItem = await _context.Carts.FirstOrDefaultAsync(c => c.Id == id);
            if (cartItem == null)
            {
                return NotFound();
            }
            // Nếu số lượng <= 0, xóa sản phẩm khỏi giỏ
            if (quantity <= 0)
            {
                _context.Carts.Remove(cartItem);
            }
            else
            {
                cartItem.Quantity = quantity;
            }
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        // Xóa sản phẩm khỏi giỏ hàng
        [HttpPost]
        public async Task<IActionResult> RemoveFromCart(int id)
        {
            var cartItem = await _context.Carts.FirstOrDefaultAsync(c => c.Id == id);
            if (cartItem == null)
            {
                return NotFound();
            }
            _context.Carts.Remove(cartItem);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }
    }
}
