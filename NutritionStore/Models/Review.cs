using System.ComponentModel.DataAnnotations;

public class Review
{
    public int Id { get; set; }

    [Required]
    public string Content { get; set; } = null!;

    public int ProductId { get; set; }

    public string UserId { get; set; } = null!;

    public DateTime CreatedOn { get; set; } = DateTime.Now;
}
