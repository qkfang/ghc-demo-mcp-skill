using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using MovieApi.Models;
using MovieApi.Repositories;

namespace MovieApi.Functions;

public sealed class BookTicketsFunction(IMovieRepository movieRepository)
{
    [Function("BookTickets")]
    public async Task<IActionResult> Run(
        [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "movies/{m_id}")]
        HttpRequest request,
        string m_id,
        CancellationToken cancellationToken)
    {
        if (!int.TryParse(m_id, out var movieId) || movieId <= 0)
        {
            return BadRequest();
        }

        if (!TryReadTicketCount(request, out var ticketCount))
        {
            return BadRequest();
        }

        var booking = await movieRepository.BookTicketsAsync(movieId, ticketCount, cancellationToken);

        return booking.Status switch
        {
            BookTicketsStatus.Success => new OkObjectResult(new[] { booking.Order! }),
            BookTicketsStatus.MovieNotFound => new NotFoundObjectResult(new MessageResponse("Resource not found")),
            BookTicketsStatus.NotEnoughTickets => new BadRequestObjectResult(
                new[]
                {
                    new ErrorResponse(
                        $"avaible tickets is only {booking.AvailableTickets} but you have ordered {ticketCount}")
                }),
            _ => BadRequest()
        };
    }

    private static bool TryReadTicketCount(HttpRequest request, out int ticketCount)
    {
        ticketCount = 0;
        var value = request.Query["no_tickets"].ToString();
        return !string.IsNullOrWhiteSpace(value) && int.TryParse(value, out ticketCount) && ticketCount > 0;
    }

    private static BadRequestObjectResult BadRequest() => new(new MessageResponse("Bad request"));
}
