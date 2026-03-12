using Microsoft.JSInterop;
using System.Text.Json;
using Metrobones.Models;

namespace Metrobones.Services;

public class LocalStorage(IJSRuntime js)
{
    private const string ClickTracksKey = "clicktracks";

    public async Task SetAsync<T>(string key, T value)
        => await js.InvokeVoidAsync("localStorage.setItem", key, JsonSerializer.Serialize(value));

    public async Task<T?> GetAsync<T>(string key)
    {
        var json = await js.InvokeAsync<string?>("localStorage.getItem", key);
        return json is null ? default : JsonSerializer.Deserialize<T>(json);
    }

    public async Task RemoveAsync(string key)
        => await js.InvokeVoidAsync("localStorage.removeItem", key);

    public async Task<List<ClickTrack>> GetAllClickTracksAsync()
        => await GetAsync<List<ClickTrack>>(ClickTracksKey) ?? new List<ClickTrack>();

    public async Task SaveClickTracksAsync(List<ClickTrack> tracks)
        => await SetAsync(ClickTracksKey, tracks);
}