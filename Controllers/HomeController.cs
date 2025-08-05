// using Microsoft.AspNetCore.Mvc;

// namespace WebRestaurant.Controllers
// {
//     public class HomeController : Controller
//     {
//         public IActionResult Index()
//         {
//             return View();
//         }
//     }
// }


using Microsoft.AspNetCore.Mvc;
using Prometheus; //  thêm thư viện Prometheus

namespace WebRestaurant.Controllers
{
    public class HomeController : Controller
    {
        // Tạo bộ đếm Prometheus
        private static readonly Counter HomePageCounter = Metrics.CreateCounter(
            "home_page_requests_total", "Tổng số lần truy cập trang Home");
        public IActionResult Index()
        {
            HomePageCounter.Inc(); // Tăng bộ đếm mỗi lần truy cập
            return View();
        }
    }
}

