using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MovieApi.Functions;
using MovieApi.Models;
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
        var message = Assert.IsType<MessageResponse>(badRequest.Value);
        Assert.Equal("Bad request", message.Message);
    }

    [Fact]
    public async Task BookTickets_ReturnsNotFound_ForUnknownMovie()
    {
        var function = new BookTicketsFunction(new InMemoryMovieRepository());
        var request = new DefaultHttpContext().Request;
        request.QueryString = new QueryString("?no_tickets=1");

        var result = await function.Run(request, "99", CancellationToken.None);

        var notFound = Assert.IsType<NotFoundObjectResult>(result);
        var message = Assert.IsType<MessageResponse>(notFound.Value);
        Assert.Equal("Resource not found", message.Message);
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

    [Fact]
    public async Task BookTickets_ReturnsLegacyInsufficientTicketError()
    {
        var function = new BookTicketsFunction(new InMemoryMovieRepository());
        var request = new DefaultHttpContext().Request;
        request.QueryString = new QueryString("?no_tickets=50");

        var result = await function.Run(request, "1", CancellationToken.None);

        var badRequest = Assert.IsType<BadRequestObjectResult>(result);
        var errors = Assert.IsAssignableFrom<IEnumerable<ErrorResponse>>(badRequest.Value);
        var error = Assert.Single(errors);
        Assert.Equal("avaible tickets is only 20 but you have ordered 50", error.Error);
    }
}
