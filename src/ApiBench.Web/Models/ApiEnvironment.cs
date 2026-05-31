namespace ApiBench.Web.Models;

public class ApiEnvironment
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Name { get; set; } = string.Empty;
    public List<KeyValueItem> Variables { get; set; } = new();
    public bool IsActive { get; set; }
}
