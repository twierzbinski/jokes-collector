namespace JokesCollector.Tests;

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Moq;
using Xunit;
using JokesCollector.Data;
using JokesCollector.Domain;
using JokesCollector.Services;

public class JokesServiceTests
{
    private const string ProviderName = "ChuckNorris";

    private readonly Mock<IJokesRepository> _repoMock;
    private readonly Mock<IJokesProvider> _providerMock;
    private readonly IJokesService _jokesService;

    public JokesServiceTests()
    {
        _repoMock = new Mock<IJokesRepository>();
        _providerMock = new Mock<IJokesProvider>();

        // Ensure provider name is defined for dictionary lookup
        _providerMock.Setup(p => p.Name).Returns(ProviderName);

        var providers = new[] { _providerMock.Object };
        _jokesService = new JokesService(_repoMock.Object, providers);
    }

    [Fact]
    public async Task CollectAndStoreJokesAsync_AddsOnlyNewFilteredAndUniqueJokes()
    {
        // Arrange
        var jokes = new List<Joke>
        {
            new ChuckNorrisJoke { Id = "1", Value = "Short one" },
            new ChuckNorrisJoke { Id = "2", Value = new string('x', 250) }, // too long
            new ChuckNorrisJoke { Id = "3", Value = "Another short" },
            new ChuckNorrisJoke { Id = "1", Value = "Duplicate Id" } // duplicate
        };

        _providerMock
            .Setup(p => p.GetJokesAsync(It.IsAny<int>()))
            .ReturnsAsync(jokes);

        // Simulate that "3" already exists, "1" does not
        _repoMock.Setup(r => r.JokeExistsAsync(It.Is<StoredJoke>(s => s.Id == "3")))
                 .ReturnsAsync(true);
        _repoMock.Setup(r => r.JokeExistsAsync(It.Is<StoredJoke>(s => s.Id == "1")))
                 .ReturnsAsync(false);

        // Act
        await _jokesService.CollectAndStoreJokesAsync(ProviderName, jokes.Count);

        // Assert - only “1” should be added once
        _repoMock.Verify(r => r.AddJokeAsync(It.Is<StoredJoke>(s => s.Id == "1")), Times.Once);
        _repoMock.Verify(r => r.AddJokeAsync(It.Is<StoredJoke>(s => s.Id == "3")), Times.Never);
        _repoMock.Verify(r => r.AddJokeAsync(It.Is<StoredJoke>(s => s.Id == "2")), Times.Never);
    }

    [Fact]
    public async Task CollectAndStoreJokesAsync_Throws_WhenProviderNotRegistered()
    {
        // Act & Assert
        var ex = await Assert.ThrowsAsync<ArgumentException>(
            () => _jokesService.CollectAndStoreJokesAsync("Unknown", 5));
        Assert.Contains("not found", ex.Message);
    }

    [Fact]
    public async Task CollectAndStoreJokesAsync_DoesNotCallRepository_WhenNoJokes()
    {
        // Arrange empty response
        _providerMock.Setup(p => p.GetJokesAsync(It.IsAny<int>()))
                     .ReturnsAsync(new List<Joke>());

        // Act
        await _jokesService.CollectAndStoreJokesAsync(ProviderName, 5);

        // Assert: no calls to repository
        _repoMock.Verify(r => r.AddJokeAsync(It.IsAny<StoredJoke>()), Times.Never);
    }
}
