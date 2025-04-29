using System.ComponentModel.DataAnnotations;

namespace NutritionStore.Models
{
    public class Review
    {
        public int Id { get; set; }

        [Required]
        [Range(1, 5)]
        public int Rating { get; set; }

        [Required]
        [StringLength(1000)]
        public string Comment { get; set; } = null!;

        public int ProductId { get; set; }

        public string UserId { get; set; } = null!;

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public Product Product { get; set; } = null!;
    }
}
