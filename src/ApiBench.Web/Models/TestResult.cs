namespace ApiBench.Web.Models;

public class TestResult
{
    public AssertionType AssertionType { get; set; }
    public string Target { get; set; } = string.Empty;
    public string Expected { get; set; } = string.Empty;
    public string Actual { get; set; } = string.Empty;
    public bool Passed { get; set; }
    public string Message { get; set; } = string.Empty;
}
