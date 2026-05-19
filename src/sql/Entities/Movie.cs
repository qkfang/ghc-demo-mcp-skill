namespace GhcDemo.Sql.Entities;

public class Movie
{
    public int MovieId { get; set; }
    public int LegacyMovieId { get; set; }
    public required string Title { get; set; }
    public string? Language { get; set; }
    public int AvailableTickets { get; set; }
    public decimal UnitPrice { get; set; }

    public ICollection<OrderLineItem> OrderLineItems { get; set; } = new List<OrderLineItem>();
}
