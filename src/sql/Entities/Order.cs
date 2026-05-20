namespace GhcDemo.Sql.Entities;

public class Order
{
    public int Id { get; set; }

    public int MovieId { get; set; }

    public int TicketCount { get; set; }

    public decimal Price { get; set; }

    public Movie Movie { get; set; } = null!;
}
