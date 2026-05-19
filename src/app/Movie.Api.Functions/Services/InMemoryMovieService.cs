using Movie.Api.Functions.Models;

namespace Movie.Api.Functions.Services;

public class InMemoryMovieService : IMovieService
{
    private readonly object _syncRoot = new();

    private readonly List<MovieItem> _movies =
    [
        new() { m_id = 1, m_name = "Movie One", m_available = 8 },
        new() { m_id = 2, m_name = "Movie Two", m_available = 12 },
        new() { m_id = 3, m_name = "Movie Three", m_available = 0 }
    ];

    private readonly List<Order> _orders = [];

    public IReadOnlyList<MovieItem> GetAvailableMovies()
    {
        lock (_syncRoot)
        {
            return _movies
                .Where(movie => movie.m_available > 0)
                .Select(movie => new MovieItem
                {
                    m_id = movie.m_id,
                    m_name = movie.m_name,
                    m_available = movie.m_available
                })
                .ToList();
        }
    }

    public BookTicketsResult BookTickets(int movieId, int noTickets)
    {
        lock (_syncRoot)
        {
            var movie = _movies.FirstOrDefault(m => m.m_id == movieId);
            if (movie is null)
            {
                return new BookTicketsResult
                {
                    Success = false,
                    ErrorType = BookTicketsErrorType.NotFound,
                    ErrorMessage = "Resource not found"
                };
            }

            if (movie.m_available - noTickets < 0)
            {
                return new BookTicketsResult
                {
                    Success = false,
                    ErrorType = BookTicketsErrorType.InsufficientTickets,
                    ErrorMessage = $"avaible tickets is only {movie.m_available} but you have ordered {noTickets}"
                };
            }

            movie.m_available -= noTickets;
            var order = new Order
            {
                o_id = _orders.Count + 1,
                m_id = movieId,
                no_tickets = noTickets,
                price = CalculatePrice(noTickets)
            };

            _orders.Add(order);

            return new BookTicketsResult
            {
                Success = true,
                ErrorType = BookTicketsErrorType.None,
                Order = order
            };
        }
    }

    private static int CalculatePrice(int noTickets)
    {
        if (noTickets <= 5)
        {
            return noTickets * 100;
        }

        if (noTickets <= 10)
        {
            return noTickets * 90;
        }

        return noTickets * 80;
    }
}
