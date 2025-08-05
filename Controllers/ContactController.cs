using Microsoft.AspNetCore.Mvc;

namespace WebRestaurant.Controllers
{
    public class ContactController : Controller
    {
        // GET: /Contact/
        public IActionResult Index()
        {
            return View();
        }

        // POST: /Contact/Submit
        [HttpPost]
        public IActionResult Submit(string Name, string Email, string Message)
        {
            // Xử lý thông tin liên hệ (ví dụ: lưu vào cơ sở dữ liệu, gửi email, …)
            TempData["Success"] = "Cảm ơn bạn đã liên hệ. Chúng tôi sẽ phản hồi sớm.";
            return RedirectToAction("Index");
        }
    }
}
