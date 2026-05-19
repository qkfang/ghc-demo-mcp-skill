namespace GhcDemo.Sql.Entities;

public class Customer
{
    public int CustomerId { get; set; }
    public required string FirstName { get; set; }
    public required string LastName { get; set; }
    public required string Email { get; set; }
    public string? PhoneNumber { get; set; }
    public DateTime CreatedAtUtc { get; set; }

    public ICollection<Order> Orders { get; set; } = new List<Order>();
}
