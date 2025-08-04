namespace JokesCollector.Domain;

public class ChuckNorrisJoke : Joke
{
    public const string SourceName = "ChuckNorris";

    public override string Id { get; set; } = "";
    public override string Value { get; set; } = "";
    public override string Source => SourceName;
}
