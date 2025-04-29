using NutritionStore.Models;

namespace NutritionStore.ViewModels
{
    public class ProductDetailsViewModel
    {
        public Product Product { get; set; } = null!;
        public IEnumerable<Review> Reviews { get; set; } = new List<Review>();
        public double AverageRating { get; set; }
        public ReviewFormModel ReviewForm { get; set; } = new ReviewFormModel();
    }
}
