using NutritionStore.Models;

namespace NutritionStore.ViewModels
{
    public class ProductDetailsViewModel
    {
        public Product Product { get; set; } = null!;
        public IEnumerable<ProductReview> Reviews { get; set; } = new List<ProductReview>();
        public double AverageRating { get; set; }
        public ReviewFormModel ReviewForm { get; set; } = new();
    }
}
