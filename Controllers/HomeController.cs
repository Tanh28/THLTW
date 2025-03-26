using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Webbanhang.Models;
using Webbanhang.Repositories;

namespace Webbanhang.Controllers
{
    public class HomeController : Controller
    {
        private static List<string> _banners = new List<string> { "Welcome to Product Management!", "Check out our latest products!" };
        private readonly ILogger<HomeController> _logger;
        private readonly IProductRepository _productRepository;

        public HomeController(ILogger<HomeController> logger, IProductRepository productRepository)
        {
            _logger = logger;
            _productRepository = productRepository;
        }

        // Hiển thị danh sách sản phẩm
        public async Task<IActionResult> Index()
        {
            ViewBag.Banners = _banners;
            var products = await _productRepository.GetAllAsync();
            return View(products);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        public IActionResult Product()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        [HttpPost]
        public IActionResult AddBanner(string bannerText)
        {
            if (!string.IsNullOrEmpty(bannerText))
            {
                _banners.Add(bannerText); // Thêm banner mới vào danh sách
            }
            return RedirectToAction("Index");
        }
    }
}
