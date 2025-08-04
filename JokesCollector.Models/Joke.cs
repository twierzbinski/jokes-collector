namespace JokesCollector.Domain;

public abstract class Joke
{
    public abstract string Id { get; set; }

    public abstract string Value { get; set; }

    public abstract string Source { get; }
}
