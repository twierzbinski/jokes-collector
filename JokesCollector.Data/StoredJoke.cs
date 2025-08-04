namespace JokesCollector.Data;

public class StoredJoke
{
    public string Id { get; set; } = "";
    public string Value { get; set; } = "";
    public string Source { get; set; } = "";
    public string? IconUrl { get; set; }
}
