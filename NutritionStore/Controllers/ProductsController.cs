using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NutritionStore.Data;
using NutritionStore.Models;
using NutritionStore.ViewModels;

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

    // ➕ Add to Cart
    [HttpPost]
    [Authorize]
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

    // 🔎 Product Details
    [AllowAnonymous]
    public async Task<IActionResult> Details(int id)
    {
        var product = await _context.Products.Include(p => p.Category).FirstOrDefaultAsync(p => p.Id == id);
        if (product == null) return NotFound();

        var reviews = await _context.Reviews
            .Where(r => r.ProductId == id)
            .OrderByDescending(r => r.CreatedAt)
            .ToListAsync();

        var avgRating = reviews.Any() ? reviews.Average(r => r.Rating) : 0;

        var viewModel = new ProductDetailsViewModel
        {
            Product = product,
            Reviews = reviews,
            AverageRating = avgRating,
            ReviewForm = new ReviewFormModel { ProductId = id }
        };

        return View(viewModel);
    }

    // ✏️ Create Product (Admin Only)
    [Authorize(Roles = "Admin")]
    [HttpGet]
    public async Task<IActionResult> Create()
    {
        ViewBag.Categories = await _context.Categories.ToListAsync();
        return View();
    }

    [Authorize(Roles = "Admin")]
    [HttpPost]
    public async Task<IActionResult> Create(ProductFormModel model)
    {
        if (!ModelState.IsValid)
        {
            ViewBag.Categories = await _context.Categories.ToListAsync();
            return View(model);
        }

        var product = new Product
        {
            Name = model.Name,
            Description = model.Description,
            Price = model.Price,
            ImageUrl = model.ImageUrl,
            CategoryId = model.CategoryId
        };

        _context.Products.Add(product);
        await _context.SaveChangesAsync();

        return RedirectToAction(nameof(All));
    }

    // ✏️ Edit Product (Admin Only)
    [Authorize(Roles = "Admin")]
    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        var product = await _context.Products.FindAsync(id);

        if (product == null)
            return NotFound();

        var model = new ProductFormModel
        {
            Name = product.Name,
            Description = product.Description,
            Price = product.Price,
            ImageUrl = product.ImageUrl,
            CategoryId = product.CategoryId
        };

        ViewBag.Categories = await _context.Categories.ToListAsync();
        return View(model);
    }

    [Authorize(Roles = "Admin")]
    [HttpPost]
    public async Task<IActionResult> Edit(int id, ProductFormModel model)
    {
        if (!ModelState.IsValid)
        {
            ViewBag.Categories = await _context.Categories.ToListAsync();
            return View(model);
        }

        var product = await _context.Products.FindAsync(id);

        if (product == null)
            return NotFound();

        product.Name = model.Name;
        product.Description = model.Description;
        product.Price = model.Price;
        product.ImageUrl = model.ImageUrl;
        product.CategoryId = model.CategoryId;

        await _context.SaveChangesAsync();

        return RedirectToAction(nameof(All));
    }

    // ❌ Delete Product (Admin Only)
    [Authorize(Roles = "Admin")]
    [HttpPost]
    public async Task<IActionResult> Delete(int id)
    {
        var product = await _context.Products.FindAsync(id);

        if (product == null)
            return NotFound();

        _context.Products.Remove(product);
        await _context.SaveChangesAsync();

        return RedirectToAction(nameof(All));
    }
}
