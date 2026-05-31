using Microsoft.JSInterop;

namespace ApiBench.Web.Services;

public class MonacoEditorService : IAsyncDisposable
{
    private readonly IJSRuntime _jsRuntime;
    private readonly Lazy<Task<IJSObjectReference>> _moduleTask;

    public MonacoEditorService(IJSRuntime jsRuntime)
    {
        _jsRuntime = jsRuntime;
        _moduleTask = new Lazy<Task<IJSObjectReference>>(() =>
            _jsRuntime.InvokeAsync<IJSObjectReference>("import", "./js/apiBenchMonaco.js").AsTask());
    }

    public async Task<int> CreateAsync(string containerId, string language, string value, bool readOnly)
    {
        var module = await _moduleTask.Value;
        return await module.InvokeAsync<int>("create", containerId, language, value, readOnly);
    }

    public async Task<string> GetValueAsync(int editorId)
    {
        var module = await _moduleTask.Value;
        return await module.InvokeAsync<string>("getValue", editorId);
    }

    public async Task SetValueAsync(int editorId, string value)
    {
        var module = await _moduleTask.Value;
        await module.InvokeVoidAsync("setValue", editorId, value);
    }

    public async Task SetLanguageAsync(int editorId, string language)
    {
        var module = await _moduleTask.Value;
        await module.InvokeVoidAsync("setLanguage", editorId, language);
    }

    public async Task DisposeEditorAsync(int editorId)
    {
        var module = await _moduleTask.Value;
        await module.InvokeVoidAsync("disposeEditor", editorId);
    }

    public async Task OnValueChangedAsync(int editorId, DotNetObjectReference<object> dotNetRef, string callbackName)
    {
        var module = await _moduleTask.Value;
        await module.InvokeVoidAsync("onValueChanged", editorId, dotNetRef, callbackName);
    }

    public async ValueTask DisposeAsync()
    {
        if (_moduleTask.IsValueCreated)
        {
            var module = await _moduleTask.Value;
            await module.DisposeAsync();
        }
    }
}
