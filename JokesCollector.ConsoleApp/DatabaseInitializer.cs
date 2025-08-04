namespace JokesCollector.ConsoleApp;

using Microsoft.Data.Sqlite;

public static class DatabaseInitializer
{
    public static SqliteConnection CreateAndInitialize(string connectionString)
    {
        var connection = new SqliteConnection(connectionString);
        connection.Open();  // Must stay open to keep the in-memory DB alive

        using var cmd = connection.CreateCommand();
        cmd.CommandText = @"
            CREATE TABLE IF NOT EXISTS Jokes (
                Id TEXT PRIMARY KEY,
                Value TEXT NOT NULL,
                Source TEXT NOT NULL
            );";
        cmd.ExecuteNonQuery();

        return connection;
    }
}
