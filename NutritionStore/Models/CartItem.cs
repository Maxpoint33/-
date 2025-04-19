public class CartItem
{
    public int Id { get; set; }

    public int ProductId { get; set; }

    public string UserId { get; set; } = null!;

    public int Quantity { get; set; }

    public DateTime AddedOn { get; set; } = DateTime.Now;
}
