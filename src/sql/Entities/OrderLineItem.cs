namespace GhcDemo.Sql.Entities;

public class OrderLineItem
{
    public int OrderLineItemId { get; set; }
    public int OrderId { get; set; }
    public int MovieId { get; set; }
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal LineTotal { get; set; }

    public Order Order { get; set; } = null!;
    public Movie Movie { get; set; } = null!;
}
