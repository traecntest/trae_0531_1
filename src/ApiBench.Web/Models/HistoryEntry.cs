namespace ApiBench.Web.Models;

public class HistoryEntry
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public ApiRequest Request { get; set; } = new();
    public ApiResponse Response { get; set; } = new();
    public string Group { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
}
