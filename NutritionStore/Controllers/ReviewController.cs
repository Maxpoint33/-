using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

[Authorize]
public class ReviewsController : Controller
{
    private readonly ApplicationDbContext _context;

    public ReviewsController(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpPost]
    public async Task<IActionResult> Create(int productId, int rating, string comment)
    {
        var userId = User.Identity?.Name;
        if (userId == null) return Unauthorized();

        var alreadyReviewed = await _context.ProductReviews
            .AnyAsync(r => r.ProductId == productId && r.UserId == userId);

        if (alreadyReviewed)
        {
            TempData["ErrorMessage"] = "⚠️ You already reviewed this product.";
            return RedirectToAction("Details", "Products", new { id = productId });
        }

        var review = new ProductReview
        {
            ProductId = productId,
            UserId = userId,
            Rating = rating,
            Comment = comment,
            PostedOn = DateTime.Now
        };

        _context.ProductReviews.Add(review);
        await _context.SaveChangesAsync();

        TempData["SuccessMessage"] = "✅ Review submitted!";
        return RedirectToAction("Details", "Products", new { id = productId });
    }
}
