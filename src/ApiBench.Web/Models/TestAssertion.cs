using System.Text.Json.Serialization;

namespace ApiBench.Web.Models;

public class TestAssertion
{
    public AssertionType Type { get; set; }
    public string Target { get; set; } = string.Empty;
    public string Expected { get; set; } = string.Empty;
    public AssertionOperator Operator { get; set; }
}

[JsonConverter(typeof(JsonStringEnumConverter<AssertionType>))]
public enum AssertionType
{
    StatusCode,
    ResponseTime,
    JsonPath,
    Regex,
    HeaderExists
}

[JsonConverter(typeof(JsonStringEnumConverter<AssertionOperator>))]
public enum AssertionOperator
{
    Equals,
    NotEquals,
    Contains,
    GreaterThan,
    LessThan
}
