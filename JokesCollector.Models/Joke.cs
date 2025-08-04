namespace JokesCollector.Domain;

// remove json attributes, please, what is it you don't understand???
public abstract class Joke
{
    public abstract string Id { get; set; }

    public abstract string Value { get; set; }

    public abstract string Source { get; }
}
