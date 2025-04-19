using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NutritionStore.Models;

[Authorize]
public class CartController : Controller
{
    public IActionResult Index()
    {
        var mockCart = new List<CartItem>
        {
            new CartItem { Id = 1, ProductId = 1, Quantity = 2, UserId = "demo", AddedOn = DateTime.Now },
            new CartItem { Id = 2, ProductId = 2, Quantity = 1, UserId = "demo", AddedOn = DateTime.Now }
        };

        ViewBag.Products = new List<Product>
        {
            new Product { Id = 1, Name = "Whey Protein", Price = 49.99m, ImageUrl = "/images/whey.jpg" },
            new Product { Id = 2, Name = "Creatine", Price = 24.99m, ImageUrl = "/images/creatine.jpg" }
        };

        return View(mockCart);
    }
}
