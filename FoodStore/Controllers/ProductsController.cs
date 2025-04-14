using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

public class ProductsController : Controller
{
    private readonly ApplicationDbContext _context;

    public ProductsController(ApplicationDbContext context)
    {
        _context = context;
    }

    public IActionResult Index()
    {
        var products = _context.Products.Include(p => p.Category).ToList();
        return View(products);
    }
}