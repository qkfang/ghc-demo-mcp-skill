namespace GhcDemo.Sql.Entities;

public class Movie
{
    public int Id { get; set; }

    public int AvailableTickets { get; set; }

    public ICollection<Order> Orders { get; set; } = [];
}
