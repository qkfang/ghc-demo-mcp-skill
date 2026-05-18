using System.Text.Json.Serialization;

namespace App.Functions.Models;

public sealed class Movie
{
    [JsonPropertyName("m_id")]
    public int Id { get; init; }

    [JsonPropertyName("m_name")]
    public string Name { get; init; } = string.Empty;

    [JsonPropertyName("m_available")]
    public int AvailableTickets { get; set; }
}
