namespace JokesCollector.Tests;

using JokesCollector.Data;
using Microsoft.Data.Sqlite;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

public class JokesRepositoryTests
{
    private SqliteConnection CreateInMemoryDatabase()
    {
        var connection = new SqliteConnection("DataSource=:memory:");
        connection.Open();
        var command = connection.CreateCommand();
        command.CommandText =
        @"
            CREATE TABLE Jokes (
                Id TEXT PRIMARY KEY,
                Value TEXT NOT NULL,
                Source TEXT NOT NULL
            );
        ";
        command.ExecuteNonQuery();
        return connection;
    }

    [Fact]
    public async Task AddJokeAsync_ShouldAddJoke()
    {
        // Arrange
        using var connection = CreateInMemoryDatabase();
        var repository = new JokesRepository(connection);
        var joke = new StoredJoke { Id = "1", Value = "Why did the chicken cross the road?", Source = "Anonymous" };

        // Act
        await repository.AddJokeAsync(joke);

        // Assert
        var jokes = await repository.GetJokesAsync(10);
        Assert.Contains(jokes, j => j.Id == joke.Id && j.Value == joke.Value && j.Source == joke.Source);
    }

    [Fact]
    public async Task JokeExistsAsync_ShouldReturnTrueIfJokeExists()
    {
        // Arrange
        using var connection = CreateInMemoryDatabase();
        var repository = new JokesRepository(connection);
        var joke = new StoredJoke { Id = "1", Value = "Why did the chicken cross the road?", Source = "Anonymous" };
        await repository.AddJokeAsync(joke);

        // Act
        var exists = await repository.JokeExistsAsync(joke);

        // Assert
        Assert.True(exists);
    }

    [Fact]
    public async Task JokeExistsAsync_ShouldReturnFalseIfJokeDoesNotExist()
    {
        // Arrange
        using var connection = CreateInMemoryDatabase();
        var repository = new JokesRepository(connection);
        var joke = new StoredJoke { Id = "1", Value = "Why did the chicken cross the road?", Source = "Anonymous" };

        // Act
        var exists = await repository.JokeExistsAsync(joke);

        // Assert
        Assert.False(exists);
    }

    [Fact]
    public async Task GetJokesAsync_ShouldReturnJokes()
    {
        // Arrange
        using var connection = CreateInMemoryDatabase();
        var repository = new JokesRepository(connection);
        var jokes = new List<StoredJoke>
        {
            new StoredJoke { Id = "1", Value = "Why did the chicken cross the road?", Source = "Anonymous" },
            new StoredJoke { Id = "2", Value = "To get to the other side.", Source = "Anonymous" }
        };
        foreach (var joke in jokes)
        {
            await repository.AddJokeAsync(joke);
        }

        // Act
        var result = await repository.GetJokesAsync(10);

        // Assert
        Assert.Equal(2, result.Count());
        Assert.Contains(result, j => j.Id == "1");
        Assert.Contains(result, j => j.Id == "2");
    }
}
