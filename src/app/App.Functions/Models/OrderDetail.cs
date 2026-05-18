using System.Text.Json.Serialization;

namespace App.Functions.Models;

public sealed class OrderDetail
{
    [JsonPropertyName("o_id")]
    public int OrderId { get; init; }

    [JsonPropertyName("m_id")]
    public int MovieId { get; init; }

    [JsonPropertyName("no_tickets")]
    public int NoTickets { get; init; }

    [JsonPropertyName("price")]
    public int Price { get; init; }
}
