namespace ApiBench.Web.Models;

public class ApiRequest
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Name { get; set; } = string.Empty;
    public string Method { get; set; } = "GET";
    public string Url { get; set; } = string.Empty;
    public List<KeyValueItem> PathParams { get; set; } = new();
    public List<KeyValueItem> QueryParams { get; set; } = new();
    public List<KeyValueItem> Headers { get; set; } = new();
    public BodyContentType BodyType { get; set; } = BodyContentType.None;
    public string RawBody { get; set; } = string.Empty;
    public List<KeyValueItem> FormFields { get; set; } = new();
    public string BinaryFileBase64 { get; set; } = string.Empty;
    public string BinaryFileName { get; set; } = string.Empty;
    public Guid? CollectionId { get; set; }
    public string PreRequestScript { get; set; } = string.Empty;
    public string TestScript { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
