using Movie.Api.Functions.Models;

namespace Movie.Api.Functions.Services;

public interface IMovieService
{
    IReadOnlyList<MovieItem> GetAvailableMovies();

    BookTicketsResult BookTickets(int movieId, int noTickets);
}

public enum BookTicketsErrorType
{
    None,
    NotFound,
    InsufficientTickets
}

public class BookTicketsResult
{
    public bool Success { get; init; }

    public Order? Order { get; init; }

    public string? ErrorMessage { get; init; }

    public BookTicketsErrorType ErrorType { get; init; }
}
