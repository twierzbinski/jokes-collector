namespace JokesCollector.Services;

using JokesCollector.Domain;

public interface IJokesProvider
{
    string Name { get; }
    Task<IEnumerable<Joke>> GetJokesAsync(int numberOfJokesToFetch);
}
