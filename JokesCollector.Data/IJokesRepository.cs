namespace JokesCollector.Data;

public interface IJokesRepository
{
    Task AddJokeAsync(StoredJoke joke);
    Task<bool> JokeExistsAsync(StoredJoke joke);
    Task<IEnumerable<StoredJoke>> GetJokesAsync(int limit);
}
