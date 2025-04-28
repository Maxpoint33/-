using System.ComponentModel.DataAnnotations;

namespace NutritionStore.Models
{
    public class ProductFormModel
    {
        [Required(ErrorMessage = "Product Name is required.")]
        [MaxLength(100, ErrorMessage = "Product Name can't be longer than 100 characters.")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Description is required.")]
        [MaxLength(1000, ErrorMessage = "Description can't be longer than 1000 characters.")]
        public string Description { get; set; } = string.Empty;

        [Range(0.01, 9999, ErrorMessage = "Price must be between 0.01 and 9999.")]
        public decimal Price { get; set; }

        [Required(ErrorMessage = "Image URL is required.")]
        [Display(Name = "Image URL")]
        public string ImageUrl { get; set; } = string.Empty;

        [Required(ErrorMessage = "Please select a category.")]
        [Display(Name = "Category")]
        public int CategoryId { get; set; }
    }
}
