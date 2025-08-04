namespace JokesCollector.Services;

using JokesCollector.Data;

public class JokesService : IJokesService
{
    private readonly IJokesRepository _jokesRepository;
    private readonly Dictionary<string, IJokesProvider> _jokesProviders;

    public JokesService(IJokesRepository jokesRepository, IEnumerable<IJokesProvider> jokesProviders)
    {
        _jokesRepository = jokesRepository ?? throw new ArgumentNullException(nameof(jokesRepository));
        _jokesProviders = jokesProviders?.ToDictionary(p => p.Name, StringComparer.OrdinalIgnoreCase)
                     ?? throw new ArgumentNullException(nameof(jokesProviders));
    }

    public async Task CollectAndStoreJokesAsync(string providerName, int numberOfJokesToFetch)
    {
        if(!_jokesProviders.TryGetValue(providerName, out var jokesProvider))
            throw new ArgumentException($"Joke provider '{providerName}' not found.");

        var jokes = await jokesProvider.GetJokesAsync(numberOfJokesToFetch);

        // Filter jokes > 200 characters and deduplicate by ID
        var filtered = jokes
            .Where(j => j.Value.Length <= 200)
            .GroupBy(j => j.Id)
            .Select(g => g.First())
            .ToList();

        foreach (var jokeContent in filtered)
        {
            var storedJoke = new StoredJoke
            {
                Id = jokeContent.Id,
                Value = jokeContent.Value,
                Source = jokeContent.Source
            };

            if (!await _jokesRepository.JokeExistsAsync(storedJoke))
            {
                await _jokesRepository.AddJokeAsync(storedJoke);
            }
        }
    }
}
