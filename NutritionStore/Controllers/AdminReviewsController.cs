using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NutritionStore.Data;

[Authorize(Roles = "Admin")]
public class AdminReviewsController : Controller
{
    private readonly ApplicationDbContext _context;

    public AdminReviewsController(ApplicationDbContext context)
    {
        _context = context;
    }

    // 📋 View all reviews
    public async Task<IActionResult> Index()
    {
        var reviews = await _context.ProductReviews
            .Include(r => r.Product)
            .OrderByDescending(r => r.PostedOn)
            .ToListAsync();

        return View(reviews);
    }

    // ❌ Delete review
    [HttpPost]
    public async Task<IActionResult> Delete(int id)
    {
        var review = await _context.ProductReviews.FindAsync(id);
        if (review == null)
        {
            return NotFound();
        }

        _context.ProductReviews.Remove(review);
        await _context.SaveChangesAsync();

        TempData["SuccessMessage"] = "✅ Review deleted!";
        return RedirectToAction(nameof(Index));
    }
}
