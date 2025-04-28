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

        var products = await _context.Products.Include(p => p.Category).ToListAsync();
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
}
