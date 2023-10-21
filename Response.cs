namespace MultiThreadBenchmark;

public record Response
{
    public User Data { get; set; } = default!;
    public string Message { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
}
