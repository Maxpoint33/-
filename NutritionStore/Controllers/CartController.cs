using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NutritionStore.Data;
using NutritionStore.Models;

[Authorize]
public class CartController : Controller
{
    private readonly ApplicationDbContext _context;

    public CartController(ApplicationDbContext context)
    {
        _context = context;
    }

    // 🛒 Show Cart
    public async Task<IActionResult> Index()
    {
        var userId = User.Identity.Name;

        var cartItems = await _context.CartItems
            .Where(c => c.UserId == userId)
            .ToListAsync();

        var products = await _context.Products.ToListAsync();
        ViewBag.Products = products;

        return View(cartItems);
    }

    // ➕ Add to Cart
    [HttpPost]
    public async Task<IActionResult> Add(int productId, int quantity)
    {
        var userId = User.Identity.Name;

        var cartItem = await _context.CartItems
            .FirstOrDefaultAsync(c => c.ProductId == productId && c.UserId == userId);

        if (cartItem != null)
        {
            cartItem.Quantity += quantity;
        }
        else
        {
            cartItem = new CartItem
            {
                ProductId = productId,
                Quantity = quantity,
                UserId = userId,
                AddedOn = DateTime.Now
            };

            _context.CartItems.Add(cartItem);
        }

        await _context.SaveChangesAsync();

        // Update session cart count
        var cartCount = await _context.CartItems.Where(c => c.UserId == userId).SumAsync(c => c.Quantity);
        HttpContext.Session.SetInt32("CartCount", cartCount);

        TempData["Success"] = "Item added to cart!";
        return RedirectToAction("Index");
    }

    // ❌ Remove from Cart
    [HttpPost]
    public async Task<IActionResult> Remove(int id)
    {
        var cartItem = await _context.CartItems.FindAsync(id);

        if (cartItem == null)
        {
            return NotFound();
        }

        _context.CartItems.Remove(cartItem);
        await _context.SaveChangesAsync();

        return RedirectToAction("Index");
    }

    // ➕➖ Update Quantity
    [HttpPost]
    public async Task<IActionResult> UpdateQuantity(int id, string actionType)
    {
        var cartItem = await _context.CartItems.FindAsync(id);
        if (cartItem == null)
        {
            return NotFound();
        }

        if (actionType == "increase")
            cartItem.Quantity++;
        else if (actionType == "decrease" && cartItem.Quantity > 1)
            cartItem.Quantity--;

        await _context.SaveChangesAsync();
        return RedirectToAction("Index");
    }

    // ✅ Fake Checkout
    [HttpPost]
    public async Task<IActionResult> Checkout()
    {
        var userId = User.Identity.Name;
        var cartItems = _context.CartItems.Where(c => c.UserId == userId);

        _context.CartItems.RemoveRange(cartItems);
        await _context.SaveChangesAsync();

        HttpContext.Session.SetInt32("CartCount", 0);

        TempData["Success"] = "🎉 Order completed!";
        return RedirectToAction("OrderCompleted");
    }

    [HttpGet]
    public IActionResult OrderCompleted()
    {
        return View();
    }
}
