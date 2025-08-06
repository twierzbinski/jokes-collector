namespace JokesCollector.Services.Dto;

using System.Text.Json.Serialization;

// Add similar BarbraStreisandDto, if needed
public class ChuckNorrisDto
{
    [JsonPropertyName("id")]
    public string Id { get; set; } = "";

    [JsonPropertyName("value")]
    public string Value { get; set; } = "";

    [JsonPropertyName("url")]
    public string Url { get; set; } = "";
}
