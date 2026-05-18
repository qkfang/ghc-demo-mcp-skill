namespace AppMod.Data.Entities;

public class Movie
{
    public int MovieId { get; set; }

    public string Title { get; set; } = string.Empty;

    public string Genre { get; set; } = string.Empty;

    public decimal TicketPrice { get; set; }

    public int AvailableTickets { get; set; }

    public DateTime ShowTime { get; set; }

    public ICollection<Order> Orders { get; set; } = new List<Order>();
}
