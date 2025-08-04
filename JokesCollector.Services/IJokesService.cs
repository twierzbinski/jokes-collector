namespace JokesCollector.Services;

public interface IJokesService
{
    Task CollectAndStoreJokesAsync(string providerName, int numberOfJokesToFetch);
}
