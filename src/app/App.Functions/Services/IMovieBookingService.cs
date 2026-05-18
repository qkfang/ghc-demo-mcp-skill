using App.Functions.Models;

namespace App.Functions.Services;

public interface IMovieBookingService
{
    Task<IReadOnlyList<Movie>> GetAvailableMoviesAsync();

    BookingResult BookTickets(int movieId, int noTickets);
}
