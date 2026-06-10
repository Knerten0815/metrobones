using Microsoft.JSInterop;
using Metrobones.Models;
using System.Diagnostics;

namespace Metrobones.Services;

public class Metronome : IAsyncDisposable
{
    private readonly IJSRuntime _js;
    private DotNetObjectReference<Metronome>? _dotNetRef;
    private int _currentSectionsBeatCount;

    public ClickTrackSectionData Section { get; set; } = new(-1);

    public MetronomeData Data { get; set; } = new();
    public bool IsRunning { get; private set; }
    public int CurrentBeat { get; private set; }

    public event Action<int>? BeatCallback;
    public event Action? OneCallback;
    public event Action? StopCallback;

    public Metronome(IJSRuntime js)
    {
        _js = js;
    }

    public async Task Initialize()
    {
        _dotNetRef = DotNetObjectReference.Create(this);
        await _js.InvokeVoidAsync("metronome.initialize", _dotNetRef, Data.NotesPerBar);
    }

    public ValueTask DisposeAsync()
    {
        _dotNetRef?.Dispose();
        return ValueTask.CompletedTask;
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
    public async Task Update()
    {
        await _js.InvokeVoidAsync("metronome.setBpm", Data.TempoData.Tempo, Data.NotesPerBar, Data.NoteValue, Data.BeatAccents, Data.Subdivisions);
    }

    /// <summary>
    /// Used by clicktracks. Updates the running metronome on the next beat  without stopping. Sets the next beat as the 1.
    /// </summary>
    public async Task Update(MetronomeData data, int sectionBeatCount)
    {
        if(data == null)
            return;
        
        Data = data;
        _currentSectionsBeatCount = sectionBeatCount;

        if(Data.Subdivisions < 0)
        {
            Debug.Assert(Data.BeatAccents.Length == Data.NotesPerBar, "BeatAccents.Length != NotesPerBar");
        }
        else
        {
            Debug.Assert(Data.BeatAccents.Length == Data.Subdivisions, "BeatAccents.Length != Subdivisions");
        }

        if(Data.TempoData.IsAgogic)
        {
            await _js.InvokeVoidAsync("metronome.setBpm", Data.TempoData.StartTempo, Data.NotesPerBar, Data.NoteValue, Data.BeatAccents, Data.Subdivisions, true, Data.TempoData.EndTempo, sectionBeatCount);
        }
        else
        {
            await _js.InvokeVoidAsync("metronome.setBpm", Data.TempoData.Tempo, Data.NotesPerBar, Data.NoteValue, Data.BeatAccents, Data.Subdivisions, true);
        }
    }

    public async Task UpdateNotesPerBar()
    {
        Data.BeatAccents = new int[Data.Subdivisions > 0 ? Data.Subdivisions : Data.NotesPerBar];
        Data.BeatAccents[0] = 1;
        await Update();
    }

    public async Task UpdateSound(double volume, string waveform, double onBeatPitch, double offBeatPitch)
    {
        await _js.InvokeVoidAsync("metronome.setClickSound", volume, waveform, onBeatPitch, offBeatPitch);
    }

    [JSInvokable]
    public Task OnBeat(int beatNumber, double currentTempo)
    {
        CurrentBeat = beatNumber;
        Data.TempoData.Tempo = (int)Math.Round(currentTempo);
        BeatCallback?.Invoke(beatNumber);

        if(beatNumber == 1)
            OneCallback?.Invoke();
        
        return Task.CompletedTask;
    }

    [JSInvokable]
    public async Task OnMediaSessionPlay()
    {
        await Update(Data, _currentSectionsBeatCount);
        await Start();
    }

    [JSInvokable]
    public async Task OnMediaSessionStop()
    {
        await Stop();
    }
}
