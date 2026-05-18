using AppMod.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace AppMod.Data;

public class AppModDbContext(DbContextOptions<AppModDbContext> options) : DbContext(options)
{
    public DbSet<Movie> Movies => Set<Movie>();

    public DbSet<Order> Orders => Set<Order>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Movie>(entity =>
        {
            entity.ToTable("movie_table");
            entity.HasKey(m => m.MovieId);

            entity.Property(m => m.MovieId)
                .HasColumnName("m_id")
                .ValueGeneratedOnAdd();

            entity.Property(m => m.Title)
                .HasColumnName("m_title")
                .HasMaxLength(100)
                .IsRequired();

            entity.Property(m => m.Genre)
                .HasColumnName("m_genre")
                .HasMaxLength(50)
                .IsRequired();

            entity.Property(m => m.TicketPrice)
                .HasColumnName("ticket_price")
                .HasColumnType("decimal(10,2)");

            entity.Property(m => m.AvailableTickets)
                .HasColumnName("m_available");

            entity.Property(m => m.ShowTime)
                .HasColumnName("show_time")
                .HasColumnType("datetime2");

            entity.HasData(
                new Movie
                {
                    MovieId = 1,
                    Title = "Interstellar",
                    Genre = "Sci-Fi",
                    TicketPrice = 100.00m,
                    AvailableTickets = 50,
                    ShowTime = new DateTime(2025, 1, 15, 19, 30, 0, DateTimeKind.Utc)
                },
                new Movie
                {
                    MovieId = 2,
                    Title = "Inception",
                    Genre = "Thriller",
                    TicketPrice = 100.00m,
                    AvailableTickets = 38,
                    ShowTime = new DateTime(2025, 1, 15, 21, 0, 0, DateTimeKind.Utc)
                },
                new Movie
                {
                    MovieId = 3,
                    Title = "The Dark Knight",
                    Genre = "Action",
                    TicketPrice = 120.00m,
                    AvailableTickets = 25,
                    ShowTime = new DateTime(2025, 1, 16, 20, 0, 0, DateTimeKind.Utc)
                });
        });

        modelBuilder.Entity<Order>(entity =>
        {
            entity.ToTable("order_table");
            entity.HasKey(o => o.OrderId);

            entity.Property(o => o.OrderId)
                .HasColumnName("o_id")
                .ValueGeneratedOnAdd();

            entity.Property(o => o.MovieId)
                .HasColumnName("m_id");

            entity.Property(o => o.TicketQuantity)
                .HasColumnName("no_tickets");

            entity.Property(o => o.TotalPrice)
                .HasColumnName("price")
                .HasColumnType("decimal(10,2)");

            entity.Property(o => o.OrderedAt)
                .HasColumnName("ordered_at")
                .HasColumnType("datetime2");

            entity.HasOne(o => o.Movie)
                .WithMany(m => m.Orders)
                .HasForeignKey(o => o.MovieId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasData(
                new Order
                {
                    OrderId = 1,
                    MovieId = 1,
                    TicketQuantity = 2,
                    TotalPrice = 200.00m,
                    OrderedAt = new DateTime(2025, 1, 10, 10, 15, 0, DateTimeKind.Utc)
                },
                new Order
                {
                    OrderId = 2,
                    MovieId = 2,
                    TicketQuantity = 1,
                    TotalPrice = 100.00m,
                    OrderedAt = new DateTime(2025, 1, 10, 11, 0, 0, DateTimeKind.Utc)
                },
                new Order
                {
                    OrderId = 3,
                    MovieId = 3,
                    TicketQuantity = 4,
                    TotalPrice = 480.00m,
                    OrderedAt = new DateTime(2025, 1, 10, 12, 30, 0, DateTimeKind.Utc)
                });
        });
    }
}
