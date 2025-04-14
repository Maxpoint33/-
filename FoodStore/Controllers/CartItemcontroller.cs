using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

public class CartItemsController : Controller
{
    private const string CartSessionKey = "Cart";
    private readonly ApplicationDbContext _context;

    public CartItemsController(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpPost]
    public IActionResult AddToCart(int productId, int quantity = 1)
    {
        var cart = HttpContext.Session.Get<List<CartItem>>(CartSessionKey) ?? new List<CartItem>();
        var existingItem = cart.FirstOrDefault(ci => ci.ProductId == productId);

        if (existingItem != null)
            existingItem.Quantity += quantity;
        else
            cart.Add(new CartItem { ProductId = productId, Quantity = quantity });

        HttpContext.Session.Set(CartSessionKey, cart);
        return RedirectToAction("ViewCart");
    }

    public IActionResult ViewCart()
    {
        var cart = HttpContext.Session.Get<List<CartItem>>(CartSessionKey) ?? new List<CartItem>();
        var products = _context.Products
            .Where(p => cart.Select(ci => ci.ProductId).Contains(p.Id))
            .ToList();

        var viewModel = cart.Select(ci => new CartViewModel
        {
            Product = products.First(p => p.Id == ci.ProductId),
            Quantity = ci.Quantity
        }).ToList();

        return View(viewModel);
    }
}