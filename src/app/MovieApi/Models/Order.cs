using System.Text.Json.Serialization;

namespace MovieApi.Models;

public sealed record Order(
    [property: JsonPropertyName("o_id")] int Id,
    [property: JsonPropertyName("m_id")] int MovieId,
    [property: JsonPropertyName("no_tickets")] int NumberOfTickets,
    [property: JsonPropertyName("price")] int Price);
