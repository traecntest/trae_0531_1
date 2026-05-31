using System.Text;
using ApiBench.Web.Models;

namespace ApiBench.Web.Services;

public class CodeGeneratorService
{
    public string Generate(ApiRequest request, CodeLanguage lang)
    {
        return lang switch
        {
            CodeLanguage.Curl => GenerateCurl(request),
            CodeLanguage.CSharp => GenerateCSharp(request),
            CodeLanguage.Python => GeneratePython(request),
            CodeLanguage.JavaScript => GenerateJavaScript(request),
            CodeLanguage.Java => GenerateJava(request),
            _ => string.Empty
        };
    }

    private string GenerateCurl(ApiRequest request)
    {
        var sb = new StringBuilder();
        sb.Append($"curl -X {request.Method} \"{request.Url}\"");

        foreach (var header in request.Headers.Where(h => h.Enabled && !string.IsNullOrEmpty(h.Key)))
        {
            sb.Append($" \\\n  -H \"{header.Key}: {header.Value}\"");
        }

        if (request.BodyType != BodyContentType.None && !string.IsNullOrEmpty(request.RawBody))
        {
            var body = request.RawBody.Replace("\"", "\\\"").Replace("\n", "\\n");
            sb.Append($" \\\n  -d \"{body}\"");
        }

        return sb.ToString();
    }

    private string GenerateCSharp(ApiRequest request)
    {
        var sb = new StringBuilder();
        sb.AppendLine("using System;");
        sb.AppendLine("using System.Net.Http;");
        sb.AppendLine("using System.Text;");
        sb.AppendLine("");
        sb.AppendLine("var client = new HttpClient();");
        sb.AppendLine($"var request = new HttpRequestMessage(HttpMethod.{GetHttpMethodName(request.Method)}, \"{request.Url}\");");
        sb.AppendLine("");

        foreach (var header in request.Headers.Where(h => h.Enabled && !string.IsNullOrEmpty(h.Key)))
        {
            sb.AppendLine($"request.Headers.Add(\"{header.Key}\", \"{header.Value}\");");
        }

        if (request.BodyType != BodyContentType.None && !string.IsNullOrEmpty(request.RawBody))
        {
            var body = request.RawBody.Replace("\"", "\\\"").Replace("\n", "\\n");
            sb.AppendLine($"request.Content = new StringContent(\"{body}\", Encoding.UTF8, \"{GetContentType(request.BodyType)}\");");
        }

        sb.AppendLine("");
        sb.AppendLine("var response = await client.SendAsync(request);");
        sb.AppendLine("response.EnsureSuccessStatusCode();");
        sb.AppendLine("var body = await response.Content.ReadAsStringAsync();");
        sb.AppendLine("Console.WriteLine(body);");

        return sb.ToString();
    }

    private string GeneratePython(ApiRequest request)
    {
        var sb = new StringBuilder();
        sb.AppendLine("import requests");
        sb.AppendLine("");

        sb.AppendLine($"url = \"{request.Url}\"");
        sb.AppendLine("");
        sb.AppendLine("headers = {");
        foreach (var header in request.Headers.Where(h => h.Enabled && !string.IsNullOrEmpty(h.Key)))
        {
            sb.AppendLine($"    \"{header.Key}\": \"{header.Value}\",");
        }
        sb.AppendLine("}");
        sb.AppendLine("");

        if (request.BodyType != BodyContentType.None && !string.IsNullOrEmpty(request.RawBody))
        {
            sb.AppendLine($"payload = '''{request.RawBody}'''");
            sb.AppendLine("");
            sb.AppendLine($"response = requests.{request.Method.ToLower()}(url, headers=headers, data=payload)");
        }
        else
        {
            sb.AppendLine($"response = requests.{request.Method.ToLower()}(url, headers=headers)");
        }

        sb.AppendLine("");
        sb.AppendLine("print(response.status_code)");
        sb.AppendLine("print(response.text)");

        return sb.ToString();
    }

    private string GenerateJavaScript(ApiRequest request)
    {
        var sb = new StringBuilder();
        sb.AppendLine("const myHeaders = new Headers();");
        foreach (var header in request.Headers.Where(h => h.Enabled && !string.IsNullOrEmpty(h.Key)))
        {
            sb.AppendLine($"myHeaders.append(\"{header.Key}\", \"{header.Value}\");");
        }
        sb.AppendLine("");

        sb.AppendLine("const requestOptions = {");
        sb.AppendLine($"  method: '{request.Method}',");
        sb.AppendLine("  headers: myHeaders,");

        if (request.BodyType != BodyContentType.None && !string.IsNullOrEmpty(request.RawBody))
        {
            sb.AppendLine($"  body: `{request.RawBody}`,");
        }

        sb.AppendLine("  redirect: 'follow'");
        sb.AppendLine("};");
        sb.AppendLine("");

        sb.AppendLine($"fetch(\"{request.Url}\", requestOptions)");
        sb.AppendLine("  .then(response => response.text())");
        sb.AppendLine("  .then(result => console.log(result))");
        sb.AppendLine("  .catch(error => console.log('error', error));");

        return sb.ToString();
    }

    private string GenerateJava(ApiRequest request)
    {
        var sb = new StringBuilder();
        sb.AppendLine("import java.io.*;");
        sb.AppendLine("import java.net.*;");
        sb.AppendLine("");
        sb.AppendLine("public class Main {");
        sb.AppendLine("  public static void main(String[] args) throws Exception {");
        sb.AppendLine($"    URL url = new URL(\"{request.Url}\");");
        sb.AppendLine($"    HttpURLConnection connection = (HttpURLConnection) url.openConnection();");
        sb.AppendLine($"    connection.setRequestMethod(\"{request.Method}\");");
        sb.AppendLine("");

        foreach (var header in request.Headers.Where(h => h.Enabled && !string.IsNullOrEmpty(h.Key)))
        {
            sb.AppendLine($"    connection.setRequestProperty(\"{header.Key}\", \"{header.Value}\");");
        }
        sb.AppendLine("");

        if (request.BodyType != BodyContentType.None && !string.IsNullOrEmpty(request.RawBody))
        {
            sb.AppendLine("    connection.setDoOutput(true);");
            sb.AppendLine("    OutputStreamWriter writer = new OutputStreamWriter(connection.getOutputStream());");
            sb.AppendLine($"    writer.write(\"{request.RawBody.Replace("\"", "\\\"").Replace("\n", "\\n")}\");");
            sb.AppendLine("    writer.flush();");
            sb.AppendLine("    writer.close();");
            sb.AppendLine("");
        }

        sb.AppendLine("    int responseCode = connection.getResponseCode();");
        sb.AppendLine("    BufferedReader in = new BufferedReader(new InputStreamReader(connection.getInputStream()));");
        sb.AppendLine("    String inputLine;");
        sb.AppendLine("    StringBuffer response = new StringBuffer();");
        sb.AppendLine("    while ((inputLine = in.readLine()) != null) { response.append(inputLine); }");
        sb.AppendLine("    in.close();");
        sb.AppendLine("    System.out.println(\"Response Code: \" + responseCode);");
        sb.AppendLine("    System.out.println(response.toString());");
        sb.AppendLine("  }");
        sb.AppendLine("}");

        return sb.ToString();
    }

    private string GetHttpMethodName(string method)
    {
        return method.ToUpper() switch
        {
            "GET" => "Get",
            "POST" => "Post",
            "PUT" => "Put",
            "DELETE" => "Delete",
            "PATCH" => "Patch",
            "HEAD" => "Head",
            "OPTIONS" => "Options",
            _ => method
        };
    }

    private string GetContentType(BodyContentType type)
    {
        return type switch
        {
            BodyContentType.Json => "application/json",
            BodyContentType.Xml => "application/xml",
            BodyContentType.Form => "application/x-www-form-urlencoded",
            BodyContentType.FormData => "multipart/form-data",
            BodyContentType.GraphQl => "application/json",
            _ => "text/plain"
        };
    }
}
