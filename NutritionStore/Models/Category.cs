using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NutritionStore.Models
{
    public class Category
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Category Name is required.")]
        [MaxLength(50, ErrorMessage = "Category Name cannot exceed 50 characters.")]
        public string Name { get; set; } = null!;

      
        public ICollection<Product> Products { get; set; } = new List<Product>();
    }
}
