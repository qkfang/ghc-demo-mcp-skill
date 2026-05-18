using App.Functions.Models;
using App.Functions.Services;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using System.Net;
using System.Text.Json;
using System.Web;

namespace App.Functions.Functions;

public sealed class MoviesFunctions(
    IMovieBookingService movieBookingService,
    ILogger<MoviesFunctions> logger)
{
    [Function(nameof(GetMovies))]
    public async Task<HttpResponseData> GetMovies(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "movies")] HttpRequestData request)
    {
        logger.LogInformation("Fetching available movies.");

        var movies = await movieBookingService.GetAvailableMoviesAsync();
        return await CreateJsonResponseAsync(request, HttpStatusCode.OK, movies);
    }

    [Function(nameof(BookTickets))]
    public async Task<HttpResponseData> BookTickets(
        [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "movies/{m_id}")] HttpRequestData request,
        string m_id)
    {
        if (!int.TryParse(m_id, out var movieId))
        {
            return await CreateErrorResponseAsync(
                request,
                HttpStatusCode.BadRequest,
                "Movie id must be a valid integer.");
        }

        var query = HttpUtility.ParseQueryString(request.Url.Query);
        if (!int.TryParse(query.Get("no_tickets"), out var noTickets) || noTickets <= 0)
        {
            return await CreateErrorResponseAsync(
                request,
                HttpStatusCode.BadRequest,
                "no_tickets query parameter must be a positive integer.");
        }

        logger.LogInformation("Booking tickets for movie {MovieId}. Tickets requested: {NoTickets}", movieId, noTickets);
        var result = movieBookingService.BookTickets(movieId, noTickets);

        if (!result.Succeeded)
        {
            return await CreateErrorResponseAsync(request, result.StatusCode, result.ErrorMessage);
        }

        return await CreateJsonResponseAsync(request, HttpStatusCode.OK, new[] { result.Order! });
    }

    private static async Task<HttpResponseData> CreateJsonResponseAsync<T>(
        HttpRequestData request,
        HttpStatusCode statusCode,
        T payload)
    {
        var response = request.CreateResponse(statusCode);
        response.Headers.Add("Content-Type", "application/json; charset=utf-8");
        await JsonSerializer.SerializeAsync(response.Body, payload);
        response.Body.Position = 0;
        return response;
    }

    private static Task<HttpResponseData> CreateErrorResponseAsync(
        HttpRequestData request,
        HttpStatusCode statusCode,
        string message)
    {
        var error = new ApiError { Error = message };
        return CreateJsonResponseAsync(request, statusCode, error);
    }
}
