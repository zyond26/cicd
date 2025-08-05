using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using WebRestaurant.Data;
using WebRestaurant.Models;

namespace WebRestaurant.Controllers
{
    [Authorize]
    public class OrderController : Controller
    {
        private readonly AppDbContext _context;
        public OrderController(AppDbContext context)
        {
            _context = context;
        }

        // GET: /Order/Checkout - Hiển thị trang thanh toán
        public async Task<IActionResult> Checkout()
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            var user = await _context.Users.FindAsync(userId);
            var viewModel = new CheckoutViewModel
            {
                Name = user.Name,
                Phone = user.Phone,
                Address = user.Address
            };

            // Sử dụng đường dẫn tuyệt đối để đảm bảo tìm thấy view Checkout
            return View("~/Views/Order/Checkout.cshtml", viewModel);
        }

        // POST: /Order/Checkout - Xử lý thanh toán và chuyển giỏ hàng thành đơn hàng
        [HttpPost]
        public async Task<IActionResult> Checkout(CheckoutViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View("~/Views/Order/Checkout.cshtml", model);
            }

            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            var cartItems = await _context.Carts.Include(c => c.MenuItem)
                                                .Where(c => c.UserId == userId)
                                                .ToListAsync();
            if (!cartItems.Any())
            {
                ModelState.AddModelError("", "Giỏ hàng trống.");
                return View("~/Views/Order/Checkout.cshtml", model);
            }

            var order = new Order
            {
                UserId = userId,
                TotalPrice = cartItems.Sum(c => c.MenuItem.Price * c.Quantity),
                Status = "pending",
                PaymentStatus = "pending",
                CreatedAt = DateTime.Now
            };
            _context.Orders.Add(order);
            await _context.SaveChangesAsync();

            foreach (var cartItem in cartItems)
            {
                var orderDetail = new OrderDetail
                {
                    OrderId = order.Id,
                    MenuItemId = cartItem.MenuItemId,
                    Quantity = cartItem.Quantity,
                    Price = cartItem.MenuItem.Price
                };
                _context.OrderDetails.Add(orderDetail);
            }
            _context.Carts.RemoveRange(cartItems);
            await _context.SaveChangesAsync();

            return RedirectToAction("Details", "Order", new { id = order.Id });
        }

        // GET: /Order/Details/{id} - Hiển thị chi tiết đơn hàng sau khi thanh toán thành công
        public async Task<IActionResult> Details(int id)
        {
            var order = await _context.Orders
                                      .Include(o => o.OrderDetails)
                                          .ThenInclude(od => od.MenuItem)
                                      .FirstOrDefaultAsync(o => o.Id == id);
            if (order == null)
            {
                return NotFound();
            }
            return View(order);
        }
    }
}
