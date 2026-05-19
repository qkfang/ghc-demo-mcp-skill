using GhcDemo.Sql.Entities;
using Microsoft.EntityFrameworkCore;

namespace GhcDemo.Sql.Data;

internal static class SeedData
{
    internal static void Seed(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Customer>().HasData(
            new Customer
            {
                CustomerId = 1,
                FirstName = "Taylor",
                LastName = "Nguyen",
                Email = "taylor.nguyen@example.com",
                PhoneNumber = "+1-555-0101",
                CreatedAtUtc = new DateTime(2025, 1, 6, 15, 30, 0, DateTimeKind.Utc)
            },
            new Customer
            {
                CustomerId = 2,
                FirstName = "Jordan",
                LastName = "Patel",
                Email = "jordan.patel@example.com",
                PhoneNumber = "+1-555-0102",
                CreatedAtUtc = new DateTime(2025, 1, 8, 9, 0, 0, DateTimeKind.Utc)
            });

        modelBuilder.Entity<Movie>().HasData(
            new Movie
            {
                MovieId = 1,
                LegacyMovieId = 1,
                Title = "Interstellar",
                Language = "English",
                AvailableTickets = 120,
                UnitPrice = 100m
            },
            new Movie
            {
                MovieId = 2,
                LegacyMovieId = 2,
                Title = "Spirited Away",
                Language = "Japanese",
                AvailableTickets = 75,
                UnitPrice = 90m
            },
            new Movie
            {
                MovieId = 3,
                LegacyMovieId = 3,
                Title = "The Dark Knight",
                Language = "English",
                AvailableTickets = 40,
                UnitPrice = 80m
            });

        modelBuilder.Entity<Order>().HasData(
            new Order
            {
                OrderId = 1,
                OrderNumber = "ORD-20250110-0001",
                CustomerId = 1,
                OrderedAtUtc = new DateTime(2025, 1, 10, 17, 0, 0, DateTimeKind.Utc),
                Status = "Confirmed",
                TotalAmount = 360m
            },
            new Order
            {
                OrderId = 2,
                OrderNumber = "ORD-20250111-0002",
                CustomerId = 1,
                OrderedAtUtc = new DateTime(2025, 1, 11, 18, 15, 0, DateTimeKind.Utc),
                Status = "Confirmed",
                TotalAmount = 500m
            },
            new Order
            {
                OrderId = 3,
                OrderNumber = "ORD-20250112-0003",
                CustomerId = 2,
                OrderedAtUtc = new DateTime(2025, 1, 12, 19, 45, 0, DateTimeKind.Utc),
                Status = "Pending",
                TotalAmount = 160m
            });

        modelBuilder.Entity<OrderLineItem>().HasData(
            new OrderLineItem
            {
                OrderLineItemId = 1,
                OrderId = 1,
                MovieId = 2,
                Quantity = 4,
                UnitPrice = 90m,
                LineTotal = 360m
            },
            new OrderLineItem
            {
                OrderLineItemId = 2,
                OrderId = 2,
                MovieId = 1,
                Quantity = 5,
                UnitPrice = 100m,
                LineTotal = 500m
            },
            new OrderLineItem
            {
                OrderLineItemId = 3,
                OrderId = 3,
                MovieId = 3,
                Quantity = 2,
                UnitPrice = 80m,
                LineTotal = 160m
            });
    }
}
