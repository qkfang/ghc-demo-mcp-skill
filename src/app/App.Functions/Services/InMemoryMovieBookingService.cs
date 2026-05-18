using App.Functions.Mappings;
using App.Functions.Models;
using App.Functions.Options;
using Microsoft.Extensions.Options;
using System.Net;
using System.Text.Json;

namespace App.Functions.Services;

public sealed class InMemoryMovieBookingService : IMovieBookingService
{
    private static readonly IReadOnlyList<Movie> DefaultMovies =
    [
        new() { Id = 1, Name = "Interstellar", AvailableTickets = 20 },
        new() { Id = 2, Name = "Inception", AvailableTickets = 10 },
        new() { Id = 3, Name = "The Matrix", AvailableTickets = 0 }
    ];

    private readonly object _gate = new();
    private readonly List<Movie> _movies;
    private readonly PricingOptions _pricing;
    private int _nextOrderId = 1;

    public InMemoryMovieBookingService(IOptions<MovieApiOptions> options)
    {
        var configuredOptions = options.Value;
        _pricing = configuredOptions.Pricing;
        _movies = ParseMovies(configuredOptions.MoviesJson).ToList();
    }

    public Task<IReadOnlyList<Movie>> GetAvailableMoviesAsync()
    {
        lock (_gate)
        {
            var payload = _movies
                .Where(movie => movie.AvailableTickets > 0)
                .Select(movie => new Movie
                {
                    Id = movie.Id,
                    Name = movie.Name,
                    AvailableTickets = movie.AvailableTickets
                })
                .ToArray();

            return Task.FromResult<IReadOnlyList<Movie>>(payload);
        }
    }

    public BookingResult BookTickets(int movieId, int noTickets)
    {
        lock (_gate)
        {
            var movie = _movies.SingleOrDefault(candidate => candidate.Id == movieId);
            if (movie is null)
            {
                return new BookingResult
                {
                    Succeeded = false,
                    StatusCode = HttpStatusCode.NotFound,
                    ErrorMessage = $"Movie with m_id {movieId} was not found."
                };
            }

            if ((movie.AvailableTickets - noTickets) < 0)
            {
                return new BookingResult
                {
                    Succeeded = false,
                    StatusCode = HttpStatusCode.BadRequest,
                    ErrorMessage = $"avaible tickets is only {movie.AvailableTickets} but you have ordered {noTickets}"
                };
            }

            movie.AvailableTickets -= noTickets;
            var order = OrderMappings.CreateOrder(_nextOrderId++, movieId, noTickets, _pricing);

            return new BookingResult
            {
                Succeeded = true,
                StatusCode = HttpStatusCode.OK,
                Order = order
            };
        }
    }

    private static IReadOnlyList<Movie> ParseMovies(string? moviesJson)
    {
        if (string.IsNullOrWhiteSpace(moviesJson))
        {
            return DefaultMovies;
        }

        var parsed = JsonSerializer.Deserialize<List<Movie>>(moviesJson);
        return parsed is { Count: > 0 } ? parsed : DefaultMovies;
    }
}
