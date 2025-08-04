namespace JokesCollector.Domain;

using System.Text.Json.Serialization;

public class ChuckNorrisJoke : Joke
{
    public const string SourceName = "ChuckNorris";

    [JsonPropertyName("url")]
    public string Url { get; set; } = "";

    public override string Source => SourceName;
}
