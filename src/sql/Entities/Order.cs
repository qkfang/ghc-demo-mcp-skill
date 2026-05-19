namespace GhcDemo.Sql.Entities;

public class Order
{
    public int OrderId { get; set; }
    public required string OrderNumber { get; set; }
    public int CustomerId { get; set; }
    public DateTime OrderedAtUtc { get; set; }
    public required string Status { get; set; }
    public decimal TotalAmount { get; set; }

    public Customer Customer { get; set; } = null!;
    public ICollection<OrderLineItem> LineItems { get; set; } = new List<OrderLineItem>();
}
