public class Order
{
    public int Id { get; set; }

    public string UserId { get; set; } = null!;

    public DateTime OrderDate { get; set; } = DateTime.Now;

    public List<OrderItem> Items { get; set; } = new();
}
