using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using ApiBench.Web;
using ApiBench.Web.Services;
using MudBlazor.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });
builder.Services.AddMudServices();
builder.Services.AddScoped<IndexedDbService>();
builder.Services.AddScoped<MonacoEditorService>();
builder.Services.AddScoped<HttpClientService>();
builder.Services.AddScoped<EnvironmentService>();
builder.Services.AddScoped<CodeGeneratorService>();
builder.Services.AddScoped<CollectionRunnerService>();

var host = builder.Build();
await host.RunAsync();
