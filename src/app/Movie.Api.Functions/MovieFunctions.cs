using System.Net;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using Movie.Api.Functions.Services;

namespace Movie.Api.Functions;

public class MovieFunctions(ILogger<MovieFunctions> logger, IMovieService movieService)
{
    private readonly ILogger<MovieFunctions> _logger = logger;
    private readonly IMovieService _movieService = movieService;

    [Function("GetMovies")]
    public async Task<HttpResponseData> GetMovies(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "movies")] HttpRequestData req)
    {
        try
        {
            var movies = _movieService.GetAvailableMovies();
            var response = req.CreateResponse(HttpStatusCode.OK);
            await response.WriteAsJsonAsync(movies);
            _logger.LogInformation("Fetched {MovieCount} available movies", movies.Count);
            return response;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to fetch movies");
            return await CreateJsonResponse(req, HttpStatusCode.InternalServerError, new { message = "Internal server error" });
        }
    }

    [Function("BookTickets")]
    public async Task<HttpResponseData> BookTickets(
        [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "movies/{m_id}")] HttpRequestData req,
        string m_id)
    {
        try
        {
            if (!int.TryParse(m_id, out var movieId) ||
                !int.TryParse(req.Query["no_tickets"], out var noTickets) ||
                noTickets <= 0)
            {
                return await CreateJsonResponse(req, HttpStatusCode.BadRequest, new { message = "Bad request" });
            }

            var result = _movieService.BookTickets(movieId, noTickets);

            if (result.Success && result.Order is not null)
            {
                var successResponse = req.CreateResponse(HttpStatusCode.OK);
                await successResponse.WriteAsJsonAsync(new[] { result.Order });
                _logger.LogInformation(
                    "Booked {TicketCount} tickets for movie {MovieId} with order {OrderId}",
                    noTickets,
                    movieId,
                    result.Order.o_id);
                return successResponse;
            }

            if (result.ErrorType == BookTicketsErrorType.InsufficientTickets)
            {
                return await CreateJsonResponse(req, HttpStatusCode.OK, new { error = result.ErrorMessage });
            }

            return await CreateJsonResponse(req, HttpStatusCode.NotFound, new { message = "Resource not found" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to book tickets for movie {MovieId}", m_id);
            return await CreateJsonResponse(req, HttpStatusCode.InternalServerError, new { message = "Internal server error" });
        }
    }

    private static async Task<HttpResponseData> CreateJsonResponse(HttpRequestData req, HttpStatusCode statusCode, object body)
    {
        var response = req.CreateResponse(statusCode);
        await response.WriteAsJsonAsync(body);
        return response;
    }
}
