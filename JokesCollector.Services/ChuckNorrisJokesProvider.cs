namespace JokesCollector.Services;

using System.Text.Json;
using JokesCollector.Domain;

public class ChuckNorrisJokesProvider : IJokesProvider
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly string _apiUrl = "https://api.chucknorris.io/jokes/random";

    public string Name => ChuckNorrisJoke.SourceName;

    public ChuckNorrisJokesProvider(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }

    public async Task<IEnumerable<Joke>> GetJokesAsync(int numberOfJokesToFetch)
    {
        var client = _httpClientFactory.CreateClient();
        var jokes = new List<ChuckNorrisJoke>();

        for (int i = 0; i < numberOfJokesToFetch; i++)
        {
            var response = await client.GetAsync(_apiUrl);
            response.EnsureSuccessStatusCode();
            var json = await response.Content.ReadAsStringAsync();
            var joke = JsonSerializer.Deserialize<ChuckNorrisJoke>(json);

            if (joke is not null && joke.Value.Length <= 200)
            {
                jokes.Add(joke);
            }
        }
        return jokes;
    }
}
