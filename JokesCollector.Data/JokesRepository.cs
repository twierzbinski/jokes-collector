namespace JokesCollector.Data;

using Dapper;
using Microsoft.Data.Sqlite;
using System.Collections.Generic;
using System.Threading.Tasks;

public class JokesRepository : IJokesRepository
{
    private readonly SqliteConnection _connection;

    public JokesRepository(SqliteConnection connection)
    {
        _connection = connection ?? throw new ArgumentException(nameof(connection));
    }
    public async Task AddJokeAsync(StoredJoke joke)
    {
        var query = "INSERT INTO Jokes (Id, Value, Source) VALUES (@Id, @Value, @Source)";
        await _connection.ExecuteAsync(query, joke);
    }

    public async Task<bool> JokeExistsAsync(StoredJoke joke)
    {
        var query = "SELECT COUNT(1) FROM Jokes WHERE Id = @Id OR Value = @Value";
        var count = await _connection.ExecuteScalarAsync<int>(query, new { joke.Id, joke.Value });
        return count > 0;
    }

    public async Task<IEnumerable<StoredJoke>> GetJokesAsync(int limit)
    {
        var query = "SELECT * FROM Jokes LIMIT @Limit";
        return await _connection.QueryAsync<StoredJoke>(query, new { Limit = limit });
    }
}
