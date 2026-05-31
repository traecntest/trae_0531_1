using System.Text.Json.Serialization;

namespace ApiBench.Web.Models;

[JsonConverter(typeof(JsonStringEnumConverter<CodeLanguage>))]
public enum CodeLanguage
{
    Curl,
    CSharp,
    Python,
    JavaScript,
    Java
}
