using Microsoft.AspNetCore.Mvc;
using Webbanhang.Models;
using Newtonsoft.Json;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

[Authorize]
public class ShoppingCartController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;

    public ShoppingCartController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    public IActionResult Index()
    {
        var cart = GetCartFromSession();
        ViewBag.CartItemCount = cart.Items.Sum(i => i.Quantity);
        return View(cart);
    }


    [HttpPost]
    public async Task<IActionResult> AddToCart(int productId, int quantity)
    {
        if (quantity <= 0) quantity = 1;

        var product = await _context.Products.FindAsync(productId);
        if (product == null) return Json(new { success = false, message = "Sản phẩm không tồn tại" });

        var cart = GetCartFromSession();
        cart.AddItem(new CartItem
        {
            ProductId = product.Id,
            Name = product.Name,
            Price = product.Price,
            Quantity = quantity
        });

        SaveCartToSession(cart);

        return Json(new { success = true, cartItemCount = cart.Items.Sum(i => i.Quantity) });
    }


    public IActionResult RemoveFromCart(int productId)
    {
        var cart = GetCartFromSession();
        cart.RemoveItem(productId);
        SaveCartToSession(cart);

        return RedirectToAction("Index");
    }


    [HttpGet]
    public async Task<IActionResult> Checkout()
    {
        var cart = GetCartFromSession();
        if (cart.Items.Count == 0)
        {
            TempData["ErrorMessage"] = "Giỏ hàng của bạn đang trống. Vui lòng thêm sản phẩm trước khi thanh toán!";
            return RedirectToAction("Index");
        }

        var user = await _userManager.GetUserAsync(User);
        if (user == null) return RedirectToAction("Login", "Account");

        var order = new Order
        {
            UserId = user.Id,
            ShippingAddress = "",
            Notes = ""
        };

        return View(order);
    }

    [HttpPost]
    public async Task<IActionResult> Checkout(Order order)
    {

        var cart = GetCartFromSession();
        if (cart.Items.Count == 0)
        {
            TempData["ErrorMessage"] = "Giỏ hàng của bạn trống!";
            return RedirectToAction("Index");
        }

        var user = await _userManager.GetUserAsync(User);
        if (user == null) return RedirectToAction("Login", "Account");

        if (string.IsNullOrWhiteSpace(order.ShippingAddress))
        {
            TempData["ErrorMessage"] = "Vui lòng nhập địa chỉ giao hàng!";
            return View(order);
        }


        order.UserId = user.Id;
        order.OrderDate = DateTime.UtcNow;
        order.TotalPrice = cart.Items.Sum(i => i.Price * i.Quantity);
        order.OrderDetails = cart.Items.Select(i => new OrderDetail
        {
            ProductId = i.ProductId,
            Quantity = i.Quantity,
            Price = i.Price
        }).ToList();

        try
        {
            _context.Orders.Add(order);
            await _context.SaveChangesAsync();


            HttpContext.Session.Remove("Cart");


            Console.WriteLine($"Đơn hàng {order.Id} đã được lưu thành công!");

            return RedirectToAction("OrderCompleted", new { orderId = order.Id });
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Lỗi Database: {ex.Message}");
            TempData["ErrorMessage"] = "Lỗi khi lưu đơn hàng. Vui lòng thử lại!";
            return View(order);
        }
    }


    public IActionResult OrderCompleted(int orderId)
    {
        ViewBag.OrderId = orderId;
        return View();
    }


    private ShoppingCart GetCartFromSession()
    {
        var cartJson = HttpContext.Session.GetString("Cart");
        return string.IsNullOrEmpty(cartJson) ? new ShoppingCart() : JsonConvert.DeserializeObject<ShoppingCart>(cartJson);
    }

 
    private void SaveCartToSession(ShoppingCart cart)
    {
        HttpContext.Session.SetString("Cart", JsonConvert.SerializeObject(cart));
    }
}