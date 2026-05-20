using System.Text.Json.Serialization;

namespace MovieApi.Models;

public sealed record Movie(
    [property: JsonPropertyName("m_id")] int Id,
    [property: JsonPropertyName("m_name")] string Name,
    [property: JsonPropertyName("m_available")] int Available);
