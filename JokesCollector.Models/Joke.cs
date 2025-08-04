namespace JokesCollector.Domain;

using System.Text.Json.Serialization;

public abstract class Joke
{
    [JsonPropertyName("id")]
    public string Id { get; set; } = "";

    [JsonPropertyName("value")]
    public string Value { get; set; } = "";

    public abstract string Source { get; }
}
