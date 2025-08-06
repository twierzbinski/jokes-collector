namespace JokesCollector.Services;

using JokesCollector.Domain;
using JokesCollector.Services.Dto;
using System.Text.Json;

public class ChuckNorrisJokesProvider : IJokesProvider
{
    private readonly HttpClient _httpClient;
    private readonly string _apiUrl = "https://api.chucknorris.io/jokes/random";

    public string Name => ChuckNorrisJoke.ProviderName;

    public ChuckNorrisJokesProvider(IHttpClientFactory factory)
        => _httpClient = factory.CreateClient(Name);

    public async Task<IEnumerable<Joke>> GetJokesAsync(int numberOfJokesToFetch)
    {
        var results = new List<Joke>(numberOfJokesToFetch);
        for (int i = 0; i < numberOfJokesToFetch; i++)
        {
            var dto = await JsonSerializer.DeserializeAsync<ChuckNorrisDto>(
                await _httpClient.GetStreamAsync(_apiUrl),
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            results.Add(new ChuckNorrisJoke
            {
                Id = dto!.Id,
                Value = dto.Value
            });
        }
        return results;
    }
}
