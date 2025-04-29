using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NutritionStore.Data;
using NutritionStore.Models;

[Authorize]
public class ReviewsController : Controller
{
    private readonly ApplicationDbContext _context;

    public ReviewsController(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpPost]
    public async Task<IActionResult> Create(ReviewFormModel model)
    {
        var userId = User.Identity?.Name;
        if (userId == null) return Unauthorized();

        var exists = await _context.Reviews.AnyAsync(r => r.ProductId == model.ProductId && r.UserId == userId);
        if (exists)
        {
            TempData["ErrorMessage"] = "You have already reviewed this product.";
            return RedirectToAction("Details", "Products", new { id = model.ProductId });
        }

        var review = new Review
        {
            ProductId = model.ProductId,
            Rating = model.Rating,
            Comment = model.Comment,
            UserId = userId,
            CreatedAt = DateTime.Now
        };

        _context.Reviews.Add(review);
        await _context.SaveChangesAsync();

        TempData["SuccessMessage"] = "✅ Review submitted!";
        return RedirectToAction("Details", "Products", new { id = model.ProductId });
    }
}
