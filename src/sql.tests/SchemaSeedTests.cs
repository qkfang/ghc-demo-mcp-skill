using GhcDemo.Sql;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace GhcDemo.Sql.Tests;

public class SchemaSeedTests
{
    [Fact]
    public async Task EnsureCreated_applies_seed_data_and_legacy_relationships()
    {
        await using var connection = new SqliteConnection("Data Source=:memory:");
        await connection.OpenAsync();

        var options = new DbContextOptionsBuilder<GhcDemoSqlContext>()
            .UseSqlite(connection)
            .Options;

        await using var context = new GhcDemoSqlContext(options);
        await context.Database.EnsureCreatedAsync();

        var movies = await context.Movies
            .OrderBy(movie => movie.Id)
            .ToListAsync();

        var orders = await context.Orders
            .Include(order => order.Movie)
            .OrderBy(order => order.Id)
            .ToListAsync();

        Assert.Collection(
            movies,
            movie =>
            {
                Assert.Equal(1, movie.Id);
                Assert.Equal(150, movie.AvailableTickets);
            },
            movie =>
            {
                Assert.Equal(2, movie.Id);
                Assert.Equal(4, movie.AvailableTickets);
            },
            movie =>
            {
                Assert.Equal(3, movie.Id);
                Assert.Equal(0, movie.AvailableTickets);
            });

        Assert.Collection(
            orders,
            order =>
            {
                Assert.Equal(1, order.Id);
                Assert.Equal(1, order.MovieId);
                Assert.Equal(2, order.TicketCount);
                Assert.Equal(200m, order.Price);
                Assert.Equal(150, order.Movie.AvailableTickets);
            },
            order =>
            {
                Assert.Equal(2, order.Id);
                Assert.Equal(2, order.MovieId);
                Assert.Equal(6, order.TicketCount);
                Assert.Equal(540m, order.Price);
                Assert.Equal(4, order.Movie.AvailableTickets);
            });
    }
}
