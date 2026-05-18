namespace AppMod.Data.Entities;

public class Order
{
    public int OrderId { get; set; }

    public int MovieId { get; set; }

    public int TicketQuantity { get; set; }

    public decimal TotalPrice { get; set; }

    public DateTime OrderedAt { get; set; }

    public Movie Movie { get; set; } = null!;
}
