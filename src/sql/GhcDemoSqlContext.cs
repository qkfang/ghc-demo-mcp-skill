using GhcDemo.Sql.Entities;
using Microsoft.EntityFrameworkCore;

namespace GhcDemo.Sql;

public class GhcDemoSqlContext(DbContextOptions<GhcDemoSqlContext> options) : DbContext(options)
{
    public DbSet<Movie> Movies => Set<Movie>();

    public DbSet<Order> Orders => Set<Order>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Movie>(entity =>
        {
            entity.ToTable("movie_table", tableBuilder =>
                tableBuilder.HasCheckConstraint("CK_movie_table_m_available", "m_available >= 0"));
            entity.HasKey(movie => movie.Id);

            entity.Property(movie => movie.Id).HasColumnName("m_id");
            entity.Property(movie => movie.AvailableTickets).HasColumnName("m_available");

            entity.HasData(
                new Movie { Id = 1, AvailableTickets = 150 },
                new Movie { Id = 2, AvailableTickets = 4 },
                new Movie { Id = 3, AvailableTickets = 0 });
        });

        modelBuilder.Entity<Order>(entity =>
        {
            entity.ToTable("order_table", tableBuilder =>
            {
                tableBuilder.HasCheckConstraint("CK_order_table_no_tickets", "no_tickets > 0");
                tableBuilder.HasCheckConstraint("CK_order_table_price", "price >= 0");
            });
            entity.HasKey(order => order.Id);

            entity.Property(order => order.Id).HasColumnName("o_id");
            entity.Property(order => order.MovieId).HasColumnName("m_id");
            entity.Property(order => order.TicketCount).HasColumnName("no_tickets");
            entity.Property(order => order.Price).HasColumnName("price").HasColumnType("decimal(10,2)");

            entity.HasIndex(order => order.MovieId);

            entity.HasOne(order => order.Movie)
                .WithMany(movie => movie.Orders)
                .HasForeignKey(order => order.MovieId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasData(
                new Order { Id = 1, MovieId = 1, TicketCount = 2, Price = 200m },
                new Order { Id = 2, MovieId = 2, TicketCount = 6, Price = 540m });
        });
    }
}
