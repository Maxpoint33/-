using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NutritionStore.Data;
using NutritionStore.Models;

public class ProductsController : Controller
{
    private readonly ApplicationDbContext _context;
    private const int PageSize = 6;

    public ProductsController(ApplicationDbContext context)
    {
        _context = context;
    }

    [AllowAnonymous]
    public async Task<IActionResult> All(string category, string searchTerm, string sort, int page = 1)
    {
        var productsQuery = _context.Products.Include(p => p.Category).AsQueryable();

        if (!string.IsNullOrEmpty(category))
            productsQuery = productsQuery.Where(p => p.Category.Name == category);

        if (!string.IsNullOrEmpty(searchTerm))
            productsQuery = productsQuery.Where(p => p.Name.Contains(searchTerm));

        if (!string.IsNullOrEmpty(sort))
        {
            productsQuery = sort switch
            {
                "priceAsc" => productsQuery.OrderBy(p => p.Price),
                "priceDesc" => productsQuery.OrderByDescending(p => p.Price),
                _ => productsQuery
            };
        }

        int totalProducts = await productsQuery.CountAsync();
        int totalPages = (int)Math.Ceiling(totalProducts / (double)PageSize);

        var products = await productsQuery
            .Skip((page - 1) * PageSize)
            .Take(PageSize)
            .ToListAsync();

        var categories = await _context.Categories.ToListAsync();

        ViewBag.Categories = categories;
        ViewBag.TotalPages = totalPages;
        ViewBag.CurrentPage = page;
        ViewBag.SelectedCategory = category;
        ViewBag.SearchTerm = searchTerm;
        ViewBag.Sort = sort;
        ViewBag.SuccessMessage = TempData["SuccessMessage"];

        return View(products);
    }

    [HttpPost]
    public async Task<IActionResult> AddToCart(int productId)
    {
        var userId = User.Identity?.Name;
        if (userId == null)
            return RedirectToAction("Login", "Account");

        var existingItem = await _context.CartItems
            .FirstOrDefaultAsync(c => c.UserId == userId && c.ProductId == productId);

        if (existingItem != null)
        {
            existingItem.Quantity++;
        }
        else
        {
            _context.CartItems.Add(new CartItem
            {
                UserId = userId,
                ProductId = productId,
                Quantity = 1,
                AddedOn = DateTime.Now
            });
        }

        await _context.SaveChangesAsync();
        TempData["SuccessMessage"] = "✅ Item added to cart!";
        return RedirectToAction(nameof(All));
    }

    [AllowAnonymous]
    public async Task<IActionResult> Details(int id)
    {
        var product = await _context.Products
            .Include(p => p.Category)
            .FirstOrDefaultAsync(p => p.Id == id);

        if (product == null)
            return NotFound();

        return View(product);
    }

    // CREATE, EDIT, DELETE are the same like before ✅
}
