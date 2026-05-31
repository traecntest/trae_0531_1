using System.Diagnostics;
using System.Net.Http.Headers;
using System.Text;
using ApiBench.Web.Models;

namespace ApiBench.Web.Services;

public class HttpClientService
{
    private readonly HttpClient _httpClient;
    private readonly IndexedDbService _indexedDbService;

    public HttpClientService(HttpClient httpClient, IndexedDbService indexedDbService)
    {
        _httpClient = httpClient;
        _indexedDbService = indexedDbService;
    }

    public async Task<ApiResponse> SendAsync(ApiRequest request, ApiEnvironment? environment = null)
    {
        var stopwatch = Stopwatch.StartNew();
        try
        {
            var url = request.Url;
            url = ResolveEnvironmentVariables(url, environment);

            foreach (var param in request.PathParams.Where(p => p.Enabled && !string.IsNullOrEmpty(p.Key)))
            {
                var value = ResolveEnvironmentVariables(param.Value, environment);
                url = url.Replace($":{param.Key}", value);
            }

            var queryParams = request.QueryParams.Where(p => p.Enabled && !string.IsNullOrEmpty(p.Key)).ToList();
            if (queryParams.Any())
            {
                var queryBuilder = new StringBuilder();
                queryBuilder.Append('?');
                foreach (var param in queryParams)
                {
                    var value = ResolveEnvironmentVariables(param.Value, environment);
                    queryBuilder.Append($"{Uri.EscapeDataString(param.Key)}={Uri.EscapeDataString(value)}&");
                }
                queryBuilder.Length--;
                url += queryBuilder.ToString();
            }

            var httpRequest = new HttpRequestMessage(new HttpMethod(request.Method), url);

            foreach (var header in request.Headers.Where(h => h.Enabled && !string.IsNullOrEmpty(h.Key)))
            {
                var value = ResolveEnvironmentVariables(header.Value, environment);
                httpRequest.Headers.TryAddWithoutValidation(header.Key, value);
            }

            var resolvedBody = ResolveEnvironmentVariables(request.RawBody, environment);

            switch (request.BodyType)
            {
                case BodyContentType.Json:
                    httpRequest.Content = new StringContent(resolvedBody, Encoding.UTF8, "application/json");
                    break;
                case BodyContentType.Xml:
                    httpRequest.Content = new StringContent(resolvedBody, Encoding.UTF8, "application/xml");
                    break;
                case BodyContentType.Form:
                    var formData = request.FormFields.Where(f => f.Enabled && !string.IsNullOrEmpty(f.Key))
                        .ToDictionary(f => f.Key, f => ResolveEnvironmentVariables(f.Value, environment));
                    httpRequest.Content = new FormUrlEncodedContent(formData);
                    break;
                case BodyContentType.FormData:
                    var multipart = new MultipartFormDataContent();
                    foreach (var field in request.FormFields.Where(f => f.Enabled && !string.IsNullOrEmpty(f.Key)))
                    {
                        var value = ResolveEnvironmentVariables(field.Value, environment);
                        multipart.Add(new StringContent(value), field.Key);
                    }
                    httpRequest.Content = multipart;
                    break;
                case BodyContentType.Binary:
                    if (!string.IsNullOrEmpty(request.BinaryFileBase64))
                    {
                        var bytes = Convert.FromBase64String(request.BinaryFileBase64);
                        httpRequest.Content = new ByteArrayContent(bytes);
                        httpRequest.Content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
                    }
                    break;
                case BodyContentType.GraphQl:
                    httpRequest.Content = new StringContent(resolvedBody, Encoding.UTF8, "application/json");
                    break;
            }

            var response = await _httpClient.SendAsync(httpRequest);
            stopwatch.Stop();

            var apiResponse = new ApiResponse
            {
                StatusCode = (int)response.StatusCode,
                StatusText = response.ReasonPhrase ?? string.Empty,
                ElapsedMs = stopwatch.ElapsedMilliseconds,
                ContentType = response.Content.Headers.ContentType?.ToString() ?? string.Empty
            };

            foreach (var header in response.Headers)
            {
                apiResponse.Headers.Add(new KeyValueItem { Key = header.Key, Value = string.Join(", ", header.Value) });
            }

            foreach (var header in response.Content.Headers)
            {
                apiResponse.Headers.Add(new KeyValueItem { Key = header.Key, Value = string.Join(", ", header.Value) });
            }

            apiResponse.Body = await response.Content.ReadAsStringAsync();

            await _indexedDbService.PutAsync("requests", request);
            await _indexedDbService.PutAsync("responses", apiResponse);

            return apiResponse;
        }
        catch (Exception ex)
        {
            stopwatch.Stop();
            return new ApiResponse
            {
                StatusCode = 0,
                StatusText = "Error",
                Body = ex.Message,
                ElapsedMs = stopwatch.ElapsedMilliseconds
            };
        }
    }

    private string ResolveEnvironmentVariables(string input, ApiEnvironment? environment)
    {
        if (string.IsNullOrEmpty(input) || environment == null)
            return input;

        foreach (var variable in environment.Variables.Where(v => v.Enabled && !string.IsNullOrEmpty(v.Key)))
        {
            input = input.Replace($"{{{{{variable.Key}}}}}", variable.Value);
        }

        return input;
    }
}
