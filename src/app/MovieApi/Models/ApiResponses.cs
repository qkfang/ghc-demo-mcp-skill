using System.Text.Json.Serialization;

namespace MovieApi.Models;

public sealed record MessageResponse([property: JsonPropertyName("message")] string Message);

public sealed record ErrorResponse([property: JsonPropertyName("error")] string Error);
