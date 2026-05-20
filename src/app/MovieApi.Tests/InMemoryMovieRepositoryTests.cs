using MovieApi.Repositories;

namespace MovieApi.Tests;

public sealed class InMemoryMovieRepositoryTests
{
    [Fact]
    public async Task GetAvailableMoviesAsync_OnlyReturnsMoviesWithTickets()
    {
        var repository = new InMemoryMovieRepository();

        var movies = await repository.GetAvailableMoviesAsync(CancellationToken.None);

        Assert.All(movies, movie => Assert.True(movie.Available > 0));
    }

    [Theory]
    [InlineData(5, 500)]
    [InlineData(8, 720)]
    [InlineData(11, 880)]
    public async Task BookTicketsAsync_CalculatesPriceByDocumentedTier(int tickets, int expectedPrice)
    {
        var repository = new InMemoryMovieRepository();

        var result = await repository.BookTicketsAsync(movieId: 1, ticketCount: tickets, CancellationToken.None);

        Assert.Equal(BookTicketsStatus.Success, result.Status);
        Assert.NotNull(result.Order);
        Assert.Equal(expectedPrice, result.Order.Price);
    }

    [Fact]
    public async Task BookTicketsAsync_ReturnsNotEnoughTicketsWhenDemandExceedsAvailability()
    {
        var repository = new InMemoryMovieRepository();

        var result = await repository.BookTicketsAsync(movieId: 2, ticketCount: 13, CancellationToken.None);

        Assert.Equal(BookTicketsStatus.NotEnoughTickets, result.Status);
        Assert.Equal(12, result.AvailableTickets);
        Assert.Null(result.Order);
    }

    [Fact]
    public async Task BookTicketsAsync_ReturnsMovieNotFoundForUnknownMovie()
    {
        var repository = new InMemoryMovieRepository();

        var result = await repository.BookTicketsAsync(movieId: 999, ticketCount: 1, CancellationToken.None);

        Assert.Equal(BookTicketsStatus.MovieNotFound, result.Status);
        Assert.Null(result.Order);
    }
}
