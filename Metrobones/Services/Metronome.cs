using Microsoft.JSInterop;
using Metrobones.Models;

namespace Metrobones.Services;

public class Metronome : IAsyncDisposable
{
    private readonly IJSRuntime _js;
    private DotNetObjectReference<Metronome>? _dotNetRef;

    public ClickTrackSection Section { get; set; } = new(-1);

    public MetronomeData Data { get; set; } = new();
    public bool IsRunning { get; private set; }
    public int CurrentBeat { get; private set; }

    public event Action? BeatCallback;
    public event Action? StopCallback;

    public Metronome(IJSRuntime js)
    {
        _js = js;
    }

    public async Task Initialize()
    {
        _dotNetRef = DotNetObjectReference.Create(this);
        await _js.InvokeVoidAsync("metronome.setDotNetReference", _dotNetRef, Data.NotesPerBar);
    }

    public async Task Start()
    {
        await _js.InvokeVoidAsync("metronome.start", Data.Tempo, Data.NotesPerBar, Data.NoteValue, Data.BeatAccents);
        IsRunning = await _js.InvokeAsync<bool>("metronome.getIsRunning");
    }

    public async Task Stop()
    {
        CurrentBeat = 0;
        await _js.InvokeVoidAsync("metronome.stop");
        IsRunning = await _js.InvokeAsync<bool>("metronome.getIsRunning");
        StopCallback?.Invoke();
    }

    /// <summary>
    /// Used by Metronome. Updates the metronome on the next beat, without stopping or resetting the beat.
    /// </summary>
    /// <returns></returns>
    public async Task UpdateSettings()
    {
        await _js.InvokeVoidAsync("metronome.setBpm", Data.Tempo, Data.NotesPerBar, Data.NoteValue, Data.BeatAccents);
    }

    /// <summary>
    /// Used by clicktracks. Updates the running metronome on the next beat  without stopping. Sets the next beat as the 1.
    /// </summary>
    public async Task UpdateSettings(MetronomeData data)
    {
        Data.Tempo = data.Tempo;
        Data.NotesPerBar = data.NotesPerBar;
        Data.NoteValue = data.NoteValue;
        Data.BeatAccents = data.BeatAccents;
        await _js.InvokeVoidAsync("metronome.setBpm", Data.Tempo, Data.NotesPerBar, Data.NoteValue, Data.BeatAccents, true);
    }

    public async Task UpdateNotesPerBar()
    {
        Data.BeatAccents = new int[Data.NotesPerBar];
        Data.BeatAccents[0] = 1;
        await UpdateSettings();
    }

    [JSInvokable]
    public Task OnBeat(int beatNumber)
    {
        CurrentBeat = beatNumber;
        BeatCallback?.Invoke();
        return Task.CompletedTask;
    }

    public ValueTask DisposeAsync()
    {
        _dotNetRef?.Dispose();
        return ValueTask.CompletedTask;
    }
}
