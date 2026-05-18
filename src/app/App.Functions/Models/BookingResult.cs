using System.Net;

namespace App.Functions.Models;

public sealed class BookingResult
{
    public bool Succeeded { get; init; }

    public HttpStatusCode StatusCode { get; init; }

    public string ErrorMessage { get; init; } = string.Empty;

    public OrderDetail? Order { get; init; }
}
