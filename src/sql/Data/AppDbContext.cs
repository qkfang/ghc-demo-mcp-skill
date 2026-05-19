using GhcDemo.Sql.Entities;
using Microsoft.EntityFrameworkCore;

namespace GhcDemo.Sql.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<Customer> Customers => Set<Customer>();
    public DbSet<Movie> Movies => Set<Movie>();
    public DbSet<Order> Orders => Set<Order>();
    public DbSet<OrderLineItem> OrderLineItems => Set<OrderLineItem>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Customer>(entity =>
        {
            entity.ToTable("customers");
            entity.HasKey(e => e.CustomerId);
            entity.Property(e => e.CustomerId).ValueGeneratedOnAdd();
            entity.Property(e => e.FirstName).HasMaxLength(100).IsRequired();
            entity.Property(e => e.LastName).HasMaxLength(100).IsRequired();
            entity.Property(e => e.Email).HasMaxLength(256).IsRequired();
            entity.Property(e => e.PhoneNumber).HasMaxLength(32);
            entity.Property(e => e.CreatedAtUtc).HasColumnType("datetime2").IsRequired();
            entity.HasIndex(e => e.Email).IsUnique();
        });

        modelBuilder.Entity<Movie>(entity =>
        {
            entity.ToTable("movies");
            entity.HasKey(e => e.MovieId);
            entity.Property(e => e.MovieId).ValueGeneratedOnAdd();
            entity.Property(e => e.LegacyMovieId).IsRequired();
            entity.Property(e => e.Title).HasMaxLength(200).IsRequired();
            entity.Property(e => e.Language).HasMaxLength(64);
            entity.Property(e => e.AvailableTickets).IsRequired();
            entity.Property(e => e.UnitPrice).HasColumnType("decimal(10,2)").IsRequired();
            entity.HasIndex(e => e.LegacyMovieId).IsUnique();
        });

        modelBuilder.Entity<Order>(entity =>
        {
            entity.ToTable("orders");
            entity.HasKey(e => e.OrderId);
            entity.Property(e => e.OrderId).ValueGeneratedOnAdd();
            entity.Property(e => e.OrderNumber).HasMaxLength(30).IsRequired();
            entity.Property(e => e.OrderedAtUtc).HasColumnType("datetime2").IsRequired();
            entity.Property(e => e.Status).HasMaxLength(32).IsRequired();
            entity.Property(e => e.TotalAmount).HasColumnType("decimal(12,2)").IsRequired();

            entity.HasIndex(e => e.OrderNumber).IsUnique();

            entity.HasOne(e => e.Customer)
                .WithMany(c => c.Orders)
                .HasForeignKey(e => e.CustomerId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<OrderLineItem>(entity =>
        {
            entity.ToTable("order_line_items");
            entity.HasKey(e => e.OrderLineItemId);
            entity.Property(e => e.OrderLineItemId).ValueGeneratedOnAdd();
            entity.Property(e => e.Quantity).IsRequired();
            entity.Property(e => e.UnitPrice).HasColumnType("decimal(10,2)").IsRequired();
            entity.Property(e => e.LineTotal).HasColumnType("decimal(12,2)").IsRequired();

            entity.HasOne(e => e.Order)
                .WithMany(o => o.LineItems)
                .HasForeignKey(e => e.OrderId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.Movie)
                .WithMany(m => m.OrderLineItems)
                .HasForeignKey(e => e.MovieId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasIndex(e => new { e.OrderId, e.MovieId }).IsUnique();
        });

        SeedData.Seed(modelBuilder);
    }
}
