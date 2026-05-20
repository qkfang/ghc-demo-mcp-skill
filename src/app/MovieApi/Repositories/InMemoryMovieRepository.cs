using MovieApi.Models;

namespace MovieApi.Repositories;

public sealed class InMemoryMovieRepository : IMovieRepository
{
    private readonly object _syncLock = new();
    private readonly List<MovieInventory> _movies =
    [
        new MovieInventory(1, "Avatar", 20),
        new MovieInventory(2, "Inception", 12),
        new MovieInventory(3, "Interstellar", 0)
    ];

    private int _nextOrderId = 1;

    public Task<IReadOnlyList<Movie>> GetAvailableMoviesAsync(CancellationToken cancellationToken)
    {
        lock (_syncLock)
        {
            var availableMovies = _movies
                .Where(movie => movie.Available > 0)
                .Select(movie => new Movie(movie.Id, movie.Name, movie.Available))
                .ToList();

            return Task.FromResult<IReadOnlyList<Movie>>(availableMovies);
        }
    }

    public Task<BookTicketsResult> BookTicketsAsync(int movieId, int ticketCount, CancellationToken cancellationToken)
    {
        lock (_syncLock)
        {
            var movie = _movies.FirstOrDefault(item => item.Id == movieId);
            if (movie is null)
            {
                return Task.FromResult(new BookTicketsResult(BookTicketsStatus.MovieNotFound, null, 0));
            }

            if (movie.Available < ticketCount)
            {
                return Task.FromResult(
                    new BookTicketsResult(BookTicketsStatus.NotEnoughTickets, null, movie.Available));
            }

            movie.Available -= ticketCount;
            var order = new Order(_nextOrderId++, movieId, ticketCount, CalculatePrice(ticketCount));

            return Task.FromResult(new BookTicketsResult(BookTicketsStatus.Success, order, movie.Available));
        }
    }

    private static int CalculatePrice(int ticketCount) =>
        ticketCount <= 5
            ? ticketCount * 100
            : ticketCount <= 10
                ? ticketCount * 90
                : ticketCount * 80;

    private sealed class MovieInventory(int id, string name, int available)
    {
        public int Id { get; } = id;

        public string Name { get; } = name;

        public int Available { get; set; } = available;
    }
}
