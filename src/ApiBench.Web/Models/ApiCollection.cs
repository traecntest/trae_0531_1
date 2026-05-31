namespace ApiBench.Web.Models;

public class ApiCollection
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public List<ApiRequest> Requests { get; set; } = new();
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
