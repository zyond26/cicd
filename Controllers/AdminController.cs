using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using WebRestaurant.Data;
using WebRestaurant.Models;

namespace WebRestaurant.Controllers
{
    [Authorize(Roles = "admin")]
    public class AdminController : Controller
    {
        private readonly AppDbContext _context;

        public AdminController(AppDbContext context)
        {
            _context = context;
        }

        // Hiển thị danh sách đơn hàng (Admin)
        public async Task<IActionResult> Index()
        {
            var orders = await _context.Orders
                .Include(o => o.User)
                .Include(o => o.OrderDetails)
                    .ThenInclude(od => od.MenuItem)
                .ToListAsync();
            return View(orders);
        }

        // Xem chi tiết đơn hàng
        public async Task<IActionResult> Details(int id)
        {
            var order = await _context.Orders
                .Include(o => o.User)
                .Include(o => o.OrderDetails)
                    .ThenInclude(od => od.MenuItem)
                .FirstOrDefaultAsync(o => o.Id == id);

            if (order == null)
            {
                return NotFound();
            }

            return View(order);
        }

        // Cập nhật trạng thái đơn hàng
        [HttpPost]
        public async Task<IActionResult> UpdateOrderStatus(int id, string status)
        {
            var order = await _context.Orders.FindAsync(id);
            if (order == null)
            {
                return NotFound();
            }
            order.Status = status;
            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = "Trạng thái đơn hàng đã được cập nhật!";
            return RedirectToAction("Index");
        }

        // Xóa đơn hàng
        [HttpPost]
        public async Task<IActionResult> DeleteOrder(int id)
        {
            var order = await _context.Orders.FindAsync(id);
            if (order == null)
            {
                return NotFound();
            }
            _context.Orders.Remove(order);
            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = "Đơn hàng đã được xóa thành công!";
            return RedirectToAction("Index");
        }

        // Quản lý món ăn
        public async Task<IActionResult> ManageMenu()
        {
            var menuItems = await _context.MenuItems.ToListAsync();
            return View(menuItems);
        }

        // Thêm món ăn mới
        [HttpGet]
        public IActionResult AddMenuItem()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> AddMenuItem(MenuItem model)
        {
            if (ModelState.IsValid)
            {
                if (!string.IsNullOrEmpty(model.ImageUrl) && Uri.IsWellFormedUriString(model.ImageUrl, UriKind.Absolute))
                {
                    _context.MenuItems.Add(model);
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = "Món ăn đã được thêm thành công!";
                    return RedirectToAction("ManageMenu");
                }
                else
                {
                    ModelState.AddModelError("ImageUrl", "URL hình ảnh không hợp lệ.");
                }
            }
            return View(model);
        }

        // Sửa món ăn
        [HttpGet]
        public async Task<IActionResult> EditMenuItem(int id)
        {
            var menuItem = await _context.MenuItems.FindAsync(id);
            if (menuItem == null)
            {
                return NotFound();
            }
            return View(menuItem);
        }

        [HttpPost]
        public async Task<IActionResult> EditMenuItem(MenuItem model)
        {
            if (ModelState.IsValid)
            {
                _context.MenuItems.Update(model);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Món ăn đã được cập nhật thành công!";
                return RedirectToAction("ManageMenu");
            }
            return View(model);
        }

        // Xóa món ăn
        [HttpPost]
        public async Task<IActionResult> DeleteMenuItem(int id)
        {
            var menuItem = await _context.MenuItems.FindAsync(id);
            if (menuItem == null)
            {
                return NotFound();
            }
            _context.MenuItems.Remove(menuItem);
            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = "Món ăn đã được xóa thành công!";
            return RedirectToAction("ManageMenu");
        }
    }
}
