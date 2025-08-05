using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebRestaurant.Data;
using WebRestaurant.Models;
using System.Linq;
using System.Threading.Tasks;

public class MenuController : Controller
{
    private readonly AppDbContext _context;
    private const int PageSize = 9; // Số món ăn hiển thị trên mỗi trang

    public MenuController(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index(string category, string sortOrder, int page = 1)
    {
        var menuItems = _context.MenuItems.AsQueryable();

        // Lọc theo danh mục món ăn
        if (!string.IsNullOrEmpty(category))
        {
            menuItems = menuItems.Where(m => m.Category == category);
        }

        // Sắp xếp theo giá
        switch (sortOrder)
        {
            case "price_asc":
                menuItems = menuItems.OrderBy(m => m.Price);
                break;
            case "price_desc":
                menuItems = menuItems.OrderByDescending(m => m.Price);
                break;
            default:
                menuItems = menuItems.OrderBy(m => m.Name); // Mặc định sắp xếp theo tên
                break;
        }

        // Phân trang
        int totalItems = await menuItems.CountAsync();
        var items = await menuItems.Skip((page - 1) * PageSize).Take(PageSize).ToListAsync();

        var viewModel = new MenuIndexViewModel
        {
            MenuItems = items,
            Categories = await _context.MenuItems.Select(m => m.Category).Distinct().ToListAsync(),
            SelectedCategory = category,
            SortOrder = sortOrder,
            CurrentPage = page,
            TotalPages = (int)System.Math.Ceiling((double)totalItems / PageSize)
        };

        return View(viewModel);
    }

    // Trang chi tiết món ăn
    public async Task<IActionResult> Details(int id)
    {
        var menuItem = await _context.MenuItems
            .Include(m => m.Reviews) // Load đánh giá
            .FirstOrDefaultAsync(m => m.Id == id);

        if (menuItem == null)
        {
            return NotFound();
        }

        return View(menuItem);
    }

    // Thêm đánh giá mới
    [HttpPost]
    public async Task<IActionResult> AddReview(int menuItemId, string userName, int rating, string comment)
    {
        if (rating < 1 || rating > 5)
        {
            ModelState.AddModelError("Rating", "Đánh giá phải từ 1 đến 5 sao.");
        }

        if (ModelState.IsValid)
        {
            var review = new Review
            {
                MenuItemId = menuItemId,
                UserName = userName,
                Rating = rating,
                Comment = comment,
                CreatedAt = DateTime.UtcNow
            };

            _context.Reviews.Add(review);
            await _context.SaveChangesAsync();
        }

        return RedirectToAction("Details", new { id = menuItemId });
    }
}
