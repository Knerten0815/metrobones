using Microsoft.JSInterop;
using Metrobones.Models;

namespace Metrobones.Services;

public class Metronome : IAsyncDisposable
{
    private readonly IJSRuntime _js;
    private DotNetObjectReference<Metronome>? _dotNetRef;

    public ClickTrackSectionData Section { get; set; } = new(-1);

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
        await _js.InvokeVoidAsync("metronome.start");
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
        await _js.InvokeVoidAsync("metronome.setBpm", Data.TempoData.Tempo, Data.NotesPerBar, Data.NoteValue, Data.BeatAccents);
    }

    /// <summary>
    /// Used by clicktracks. Updates the running metronome on the next beat  without stopping. Sets the next beat as the 1.
    /// </summary>
    public async Task UpdateSettings(MetronomeData data, int sectionBeatCount)
    {
        if(data == null)
            return;
        
        Data = data;
        if(Data.TempoData.IsAgogic)
        {
            await _js.InvokeVoidAsync("metronome.setBpm", Data.TempoData.StartTempo, Data.NotesPerBar, Data.NoteValue, Data.BeatAccents, true, Data.TempoData.EndTempo, sectionBeatCount);
        }
        else
        {
            await _js.InvokeVoidAsync("metronome.setBpm", Data.TempoData.Tempo, Data.NotesPerBar, Data.NoteValue, Data.BeatAccents, true);
        }
    }

    public async Task UpdateNotesPerBar()
    {
        Data.BeatAccents = new int[Data.NotesPerBar];
        Data.BeatAccents[0] = 1;
        await UpdateSettings();
    }

    [JSInvokable]
    public Task OnBeat(int beatNumber, double currentTempo)
    {
        CurrentBeat = beatNumber;
        Data.TempoData.Tempo = (int)Math.Round(currentTempo);
        BeatCallback?.Invoke();
        return Task.CompletedTask;
    }

    public ValueTask DisposeAsync()
    {
        _dotNetRef?.Dispose();
        return ValueTask.CompletedTask;
    }
}
