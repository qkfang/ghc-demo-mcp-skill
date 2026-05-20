using MovieApi.Models;

namespace MovieApi.Repositories;

public enum BookTicketsStatus
{
    Success,
    MovieNotFound,
    NotEnoughTickets
}

public sealed record BookTicketsResult(BookTicketsStatus Status, Order? Order, int AvailableTickets);
