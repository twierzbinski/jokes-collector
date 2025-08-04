using JokesCollector.ConsoleApp;
using JokesCollector.Data;
using JokesCollector.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using SQLitePCL;

Batteries.Init();

var configuration = new ConfigurationBuilder()
    .AddJsonFile("appsettings.json", optional: true)
    .AddEnvironmentVariables()
    .Build();

var connectionString = configuration.GetConnectionString("JokesDb");
if (string.IsNullOrEmpty(connectionString))
    throw new ArgumentNullException(nameof(connectionString));
var connection = DatabaseInitializer.CreateAndInitialize(connectionString);

var services = new ServiceCollection()
    .AddSingleton<IConfiguration>(configuration)
    .AddHttpClient()
    .AddSingleton(connection)
    .AddLogging(cfg => cfg.AddConsole())
    .AddSingleton<IJokesProvider, ChuckNorrisJokesProvider>()
    .AddSingleton<IJokesRepository, JokesRepository>()
    .AddSingleton<IJokesService, JokesService>()
    .BuildServiceProvider();

var logger = services.GetRequiredService<ILogger<Program>>();
var jokesService = services.GetRequiredService<IJokesService>();
var config = services.GetRequiredService<IConfiguration>();

var providerName = config["JokesProvider"] ?? "ChuckNorris";
if (!int.TryParse(config["NumberOfJokesToFetch"], out var numberOfJokes))
{
    numberOfJokes = 5;
}

try
{
    logger.LogInformation($"Collecting {numberOfJokes} jokes from '{providerName}'");
    await jokesService.CollectAndStoreJokesAsync(providerName, numberOfJokes);
    logger.LogInformation("Jokes collected and stored.");
}
catch (Exception ex)
{
    logger.LogError(ex, "Failed to collect and store jokes.");
}
