namespace Functions;

using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using JokesCollector.Services;

public class JokesFunction
{
    private readonly IConfiguration _configuration; // not called explicitly, but it sounds like Azure API need it here to read config
    private readonly ILogger _log;
    private readonly IJokesService _jokesService;

    public JokesFunction(
        IConfiguration configuration,
        ILogger<JokesFunction> log,
        IJokesService jokesService)
    {
        _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        _log = log ?? throw new ArgumentNullException(nameof(log));
        _jokesService = jokesService ?? throw new ArgumentNullException(nameof(jokesService));
    }

    [FunctionName("CollectJokes")]
    public async Task Run([TimerTrigger("%TimerTriggerInterval%")] TimerInfo myTimer)
    {
        _log.LogInformation($"Jokes fetching executed at: {DateTime.Now}");

        var providerName = _configuration["JokesProvider"];
        var numberOfJokesToFetch = _configuration["NumberOfJokesToFetch"];

        if (!int.TryParse(numberOfJokesToFetch, out var numberOfJokes))
        {
            _log.LogError("Invalid value for NumberOfJokesToFetch.");
            return;
        }

        if (string.IsNullOrWhiteSpace(providerName))
        {
            _log.LogError("JokesProvider is not configured.");
            return;
        }

        await _jokesService.CollectAndStoreJokesAsync(providerName, numberOfJokes);

        _log.LogInformation("Jokes collected and stored.");
    }
}
