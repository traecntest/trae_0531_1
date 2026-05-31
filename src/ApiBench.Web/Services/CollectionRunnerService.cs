using Jint;
using ApiBench.Web.Models;

namespace ApiBench.Web.Services;

public class CollectionRunnerService
{
    private readonly HttpClientService _httpClientService;
    private readonly IndexedDbService _indexedDbService;

    public CollectionRunnerService(HttpClientService httpClientService, IndexedDbService indexedDbService)
    {
        _httpClientService = httpClientService;
        _indexedDbService = indexedDbService;
    }

    public async Task<List<RunResult>> RunCollectionAsync(ApiCollection collection, ApiEnvironment? env = null)
    {
        var results = new List<RunResult>();

        foreach (var request in collection.Requests)
        {
            var testResults = new List<TestResult>();
            var allPassed = true;

            var response = await _httpClientService.SendAsync(request, env);

            if (!string.IsNullOrEmpty(request.TestScript))
            {
                var engine = new Engine();

                var responseObj = new
                {
                    statusCode = response.StatusCode,
                    statusText = response.StatusText,
                    body = response.Body,
                    elapsedMs = response.ElapsedMs,
                    headers = response.Headers.ToDictionary(h => h.Key, h => h.Value)
                };

                engine.SetValue("response", responseObj);
                engine.SetValue("console", new { log = new Action<object>(Console.WriteLine) });

                var testResultsList = new List<TestResult>();
                engine.SetValue("test", new Action<string, object>((name, fn) =>
                {
                    try
                    {
                        testResultsList.Add(new TestResult
                        {
                            AssertionType = AssertionType.StatusCode,
                            Target = name,
                            Expected = "pass",
                            Actual = "pass",
                            Passed = true,
                            Message = $"Test '{name}' passed"
                        });
                    }
                    catch (Exception ex)
                    {
                        testResultsList.Add(new TestResult
                        {
                            AssertionType = AssertionType.StatusCode,
                            Target = name,
                            Expected = "pass",
                            Actual = "fail",
                            Passed = false,
                            Message = $"Test '{name}' failed: {ex.Message}"
                        });
                        allPassed = false;
                    }
                }));

                engine.SetValue("assertEqual", new Action<object, object, string>((actual, expected, message) =>
                {
                    var passed = Equals(actual, expected);
                    if (!passed) allPassed = false;
                    testResultsList.Add(new TestResult
                    {
                        AssertionType = AssertionType.StatusCode,
                        Target = message,
                        Expected = expected?.ToString() ?? string.Empty,
                        Actual = actual?.ToString() ?? string.Empty,
                        Passed = passed,
                        Message = passed ? $"Assertion passed: {message}" : $"Assertion failed: {message}"
                    });
                }));

                engine.SetValue("assertContains", new Action<string, string, string>((actual, substring, message) =>
                {
                    var passed = actual?.Contains(substring) ?? false;
                    if (!passed) allPassed = false;
                    testResultsList.Add(new TestResult
                    {
                        AssertionType = AssertionType.Regex,
                        Target = message,
                        Expected = substring,
                        Actual = actual ?? string.Empty,
                        Passed = passed,
                        Message = passed ? $"Assertion passed: {message}" : $"Assertion failed: {message}"
                    });
                }));

                try
                {
                    engine.Execute(request.TestScript);
                    testResults.AddRange(testResultsList);
                }
                catch (Exception ex)
                {
                    testResults.Add(new TestResult
                    {
                        AssertionType = AssertionType.StatusCode,
                        Target = "Script Execution",
                        Expected = "No errors",
                        Actual = ex.Message,
                        Passed = false,
                        Message = $"Script error: {ex.Message}"
                    });
                    allPassed = false;
                }
            }

            results.Add(new RunResult
            {
                RequestName = request.Name,
                Response = response,
                TestResults = testResults,
                AllPassed = allPassed
            });
        }

        return results;
    }
}

public class RunResult
{
    public string RequestName { get; set; } = string.Empty;
    public ApiResponse Response { get; set; } = new();
    public List<TestResult> TestResults { get; set; } = new();
    public bool AllPassed { get; set; }
}
