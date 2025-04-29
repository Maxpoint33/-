using NutritionStore.Models;
using System.ComponentModel.DataAnnotations;

public class ProductReview
{
    public int Id { get; set; }

    [Required]
    public int ProductId { get; set; }

    public Product Product { get; set; } = null!;

    [Required]
    [StringLength(1000)]
    public string Comment { get; set; } = null!;

    [Range(1, 5)]
    public int Rating { get; set; }

    public string UserId { get; set; } = null!;

    public DateTime PostedOn { get; set; } = DateTime.Now;
}
