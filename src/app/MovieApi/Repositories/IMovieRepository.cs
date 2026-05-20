using MovieApi.Models;

namespace MovieApi.Repositories;

public interface IMovieRepository
{
    Task<IReadOnlyList<Movie>> GetAvailableMoviesAsync(CancellationToken cancellationToken);

    Task<BookTicketsResult> BookTicketsAsync(int movieId, int ticketCount, CancellationToken cancellationToken);
}
