using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace NutritionStore.Controllers
{
    [Authorize(Roles = "Admin")]
    public class OrdersController : Controller
    {
        public IActionResult Index()
        {
            
            return View();
        }
    }
}
