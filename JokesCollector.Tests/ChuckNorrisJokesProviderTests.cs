namespace JokesCollector.Tests;

using JokesCollector.Domain;
using JokesCollector.Services;
using Xunit;
using Moq;
using JokesCollector.Data;
using System.Collections.Generic;

public class ChuckNorrisJokesServiceTests
{
    private string ProviderName = ChuckNorrisJoke.ProviderName;
    private readonly Mock<IJokesRepository> _repo = new();
    private readonly Mock<IJokesProvider> _prov = new();
    private readonly JokesService _service;

    public ChuckNorrisJokesServiceTests()
    {
        _prov.SetupGet(x => x.Name).Returns(ProviderName);
        _service = new JokesService(_repo.Object, new[] { _prov.Object });
    }

    [Fact]
    public async Task CollectAndStoreJokesAsync_AddsFilteredUniqueJokes()
    {
        var jokes = new List<ChuckNorrisJoke>
        {
            new() { Id = "1", Value = "Short" },
            new() { Id = "2", Value = new string('x', 250) },
            new() { Id = "1", Value = "ShortDup" },
            new() { Id = "3", Value = "Also short" }
        };

        _prov.Setup(x => x.GetJokesAsync(It.IsAny<int>()))
             .ReturnsAsync(new List<Joke>(jokes));

        _repo.Setup(x => x.JokeExistsAsync(It.Is<StoredJoke>(s => s.Id == "1")))
             .ReturnsAsync(false);
        _repo.Setup(x => x.JokeExistsAsync(It.Is<StoredJoke>(s => s.Id == "3")))
             .ReturnsAsync(true);

        await _service.CollectAndStoreJokesAsync(ProviderName, jokes.Count);

        _repo.Verify(x => x.AddJokeAsync(It.Is<StoredJoke>(s => s.Id == "1")), Times.Once);
        _repo.Verify(x => x.AddJokeAsync(It.Is<StoredJoke>(s => s.Id == "3")), Times.Never);
    }
}
