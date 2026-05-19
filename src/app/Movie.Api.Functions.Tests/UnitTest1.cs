using Movie.Api.Functions.Services;

namespace Movie.Api.Functions.Tests;

public class UnitTest1
{
    [Fact]
    public void GetAvailableMovies_ReturnsOnlyMoviesWithAvailableTickets()
    {
        var service = new InMemoryMovieService();

        var movies = service.GetAvailableMovies();

        Assert.All(movies, movie => Assert.True(movie.m_available > 0));
    }

    [Fact]
    public void BookTickets_UsesTieredPricing()
    {
        var service = new InMemoryMovieService();

        var result = service.BookTickets(movieId: 2, noTickets: 6);

        Assert.True(result.Success);
        Assert.NotNull(result.Order);
        Assert.Equal(540, result.Order!.price);
    }

    [Fact]
    public void BookTickets_ReturnsInsufficientTicketsError()
    {
        var service = new InMemoryMovieService();

        var result = service.BookTickets(movieId: 1, noTickets: 100);

        Assert.False(result.Success);
        Assert.Equal(BookTicketsErrorType.InsufficientTickets, result.ErrorType);
        Assert.Contains("avaible tickets is only", result.ErrorMessage);
    }
}
