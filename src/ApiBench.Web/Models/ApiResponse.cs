namespace ApiBench.Web.Models;

public class ApiResponse
{
    public int StatusCode { get; set; }
    public string StatusText { get; set; } = string.Empty;
    public List<KeyValueItem> Headers { get; set; } = new();
    public List<KeyValueItem> Cookies { get; set; } = new();
    public string Body { get; set; } = string.Empty;
    public string ContentType { get; set; } = string.Empty;
    public long ElapsedMs { get; set; }
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
}
