using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MovieApi.Functions;
using MovieApi.Repositories;

namespace MovieApi.Tests;

public sealed class FunctionTests
{
    [Fact]
    public async Task BookTickets_ReturnsBadRequest_WhenNoTicketsQueryIsMissing()
    {
        var function = new BookTicketsFunction(new InMemoryMovieRepository());
        var request = new DefaultHttpContext().Request;

        var result = await function.Run(request, "1", CancellationToken.None);

        var badRequest = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal("Bad request", badRequest.Value?.GetType().GetProperty("message")?.GetValue(badRequest.Value));
    }

    [Fact]
    public async Task BookTickets_ReturnsNotFound_ForUnknownMovie()
    {
        var function = new BookTicketsFunction(new InMemoryMovieRepository());
        var request = new DefaultHttpContext().Request;
        request.QueryString = new QueryString("?no_tickets=1");

        var result = await function.Run(request, "99", CancellationToken.None);

        var notFound = Assert.IsType<NotFoundObjectResult>(result);
        Assert.Equal("Resource not found", notFound.Value?.GetType().GetProperty("message")?.GetValue(notFound.Value));
    }

    [Fact]
    public async Task GetMovies_ReturnsOnlyAvailableMovies()
    {
        var function = new GetMoviesFunction(new InMemoryMovieRepository());
        var request = new DefaultHttpContext().Request;

        var result = await function.Run(request, CancellationToken.None);

        var ok = Assert.IsType<OkObjectResult>(result);
        var movies = Assert.IsAssignableFrom<IReadOnlyList<MovieApi.Models.Movie>>(ok.Value);
        Assert.All(movies, movie => Assert.True(movie.Available > 0));
    }
}
