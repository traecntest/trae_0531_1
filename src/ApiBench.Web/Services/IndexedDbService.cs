using System.Text.Json;
using Microsoft.JSInterop;

namespace ApiBench.Web.Services;

public class IndexedDbService : IAsyncDisposable
{
    private readonly IJSRuntime _jsRuntime;
    private IJSObjectReference? _module;

    public IndexedDbService(IJSRuntime jsRuntime)
    {
        _jsRuntime = jsRuntime;
    }

    private async Task<IJSObjectReference> GetModuleAsync()
    {
        if (_module is null)
        {
            _module = await _jsRuntime.InvokeAsync<IJSObjectReference>("import", "./js/apiBenchIndexedDb.js");
        }
        return _module;
    }

    public async Task InitAsync()
    {
        var module = await GetModuleAsync();
        await module.InvokeVoidAsync("init", "ApiBenchDb", 1, new[] { "requests", "responses", "collections", "environments", "history" });
    }

    public async Task<T?> GetAsync<T>(string store, string id)
    {
        var module = await GetModuleAsync();
        var element = await module.InvokeAsync<JsonElement>("get", store, id);
        if (element.ValueKind == JsonValueKind.Null || element.ValueKind == JsonValueKind.Undefined)
        {
            return default;
        }
        return JsonSerializer.Deserialize<T>(element.GetRawText());
    }

    public async Task<IList<T>> GetAllAsync<T>(string store)
    {
        var module = await GetModuleAsync();
        var element = await module.InvokeAsync<JsonElement>("getAll", store);
        return JsonSerializer.Deserialize<IList<T>>(element.GetRawText()) ?? new List<T>();
    }

    public async Task PutAsync<T>(string store, T item)
    {
        var module = await GetModuleAsync();
        await module.InvokeVoidAsync("put", store, item);
    }

    public async Task DeleteAsync(string store, string id)
    {
        var module = await GetModuleAsync();
        await module.InvokeVoidAsync("delete", store, id);
    }

    public async Task ClearAsync(string store)
    {
        var module = await GetModuleAsync();
        await module.InvokeVoidAsync("clear", store);
    }

    public async ValueTask DisposeAsync()
    {
        if (_module is not null)
        {
            await _module.DisposeAsync();
        }
    }
}
