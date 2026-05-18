using System.Text.Json.Serialization;

namespace App.Functions.Models;

public sealed class ApiError
{
    [JsonPropertyName("error")]
    public string Error { get; init; } = string.Empty;
}
