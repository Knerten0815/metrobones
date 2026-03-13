using Microsoft.JSInterop;
using System.Text.Json;

namespace Metrobones.Services;

public class LocalStorage(IJSRuntime js)
{
    public async Task<T?> GetAsync<T>(string key)
    {
        var json = await js.InvokeAsync<string?>("localStorage.getItem", key);
        return json is null ? default : JsonSerializer.Deserialize<T>(json);
    }

    public async Task SetAsync<T>(string key, T value)
        => await js.InvokeVoidAsync("localStorage.setItem", key, 
            JsonSerializer.Serialize(value));

    public async Task RemoveAsync(string key)
        => await js.InvokeVoidAsync("localStorage.removeItem", key);
}

// public class LocalStorage(IJSRuntime js)
// {
//     private List<ClickTrack>? _tracks = null;
//     private const string ClickTracksKey = "clicktracks";

//     private async Task Initialize()
//     {
//         // get ClickTracks
//         _tracks = await GetAsync<List<ClickTrack>>(ClickTracksKey) ?? new DefaultTracks().Tracks;
//     }

//     public async Task<List<ClickTrack>> GetAllTracksAsync()
//     {
//         if (_tracks == null)
//         {
//             await Initialize();
//         }
//         return _tracks!;
//     }

//     public async Task<ClickTrack> GetTrackAsync(int trackID)
//     {
//         if (_tracks == null)
//         {
//             await Initialize();
//         }
//         return _tracks!.First(t => t.ID == trackID);
//     }

//     public async Task RemoveTrackAsync(int trackID)
//     {
//         if (_tracks == null)
//         {
//             await Initialize();
//         }
//         _tracks!.RemoveAll(t => t.ID == trackID);
//         await SetAsync(ClickTracksKey, _tracks);
//     }

//     public async Task SaveClickTracksAsync(List<ClickTrack> tracks)
//     {
//         _tracks = tracks;
//         await SetAsync(ClickTracksKey, tracks);
//     }

//     public async Task SetAsync<T>(string key, T value)
//         => await js.InvokeVoidAsync("localStorage.setItem", key, JsonSerializer.Serialize(value));

//     public async Task<T?> GetAsync<T>(string key)
//     {
//         var json = await js.InvokeAsync<string?>("localStorage.getItem", key);
//         return json is null ? default : JsonSerializer.Deserialize<T>(json);
//     }

//     public async Task RemoveAsync(string key)
//         => await js.InvokeVoidAsync("localStorage.removeItem", key);
// }