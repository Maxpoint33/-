using System.ComponentModel.DataAnnotations;

namespace NutritionStore.Models
{
    public class ReviewFormModel
    {
        [Required]
        [StringLength(1000, MinimumLength = 5)]
        public string Comment { get; set; } = null!;

        [Range(1, 5)]
        public int Rating { get; set; }

        public int ProductId { get; set; }
    }
}
