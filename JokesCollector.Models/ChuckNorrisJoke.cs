namespace JokesCollector.Domain;

public class ChuckNorrisJoke : Joke
{
    private const string SourceName = "ChuckNorris";

    public static string ProviderName => SourceName;

    public override string Id { get; set; } = "";
    public override string Value { get; set; } = "";
    public override string Source => SourceName;
}
