using System.Text.Json.Serialization;

namespace ApiBench.Web.Models;

[JsonConverter(typeof(JsonStringEnumConverter<BodyContentType>))]
public enum BodyContentType
{
    None,
    Json,
    Xml,
    Form,
    FormData,
    Binary,
    GraphQl
}
