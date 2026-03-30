using Metrobones.Models;

namespace Metrobones.Services;

public class ClickTrackStore(LocalStorage storage)
{
    private const string Key = "clicktracks";
    private List<ClickTrackData>? _tracks;
    private static readonly List<ClickTrackData> _defaultTracks = new List<ClickTrackData>()
    {
        new ClickTrackData() {ID=0, Title="Simple Song", Sections = new List<ClickTrackSectionData>
            {
                new ClickTrackSectionData (id: 0) { Title = "Intro", Length = 2, MetData = new MetronomeData(){TempoData = new TempoData(100)}},
                new ClickTrackSectionData (id: 1) { Title = "Verse 1", Length = 4 , MetData = new MetronomeData(){TempoData = new TempoData(120)}},
                new ClickTrackSectionData (id: 2) { Title = "Chorus", Length = 4 , MetData = new MetronomeData(){TempoData = new TempoData(100)}},
                new ClickTrackSectionData (id: 3) { Title = "Verse 2", Length = 4 , MetData = new MetronomeData(){TempoData = new TempoData(120)}},
                new ClickTrackSectionData (id: 4) { Title = "Chorus", Length = 4 , MetData = new MetronomeData(){TempoData = new TempoData(100)}},
            }
        },
        new ClickTrackData() {ID=1, Title="Weird Track", 
            Sections = new List<ClickTrackSectionData>
            {
                new ClickTrackSectionData (id: 0) { Title = "Slow Waltz", Length = 4, MetData = new MetronomeData(notesPerBar: 3) { TempoData = new TempoData(75) }},
                new ClickTrackSectionData (id: 1) { Title = "Double tempo", Length = 4, MetData = new MetronomeData(notesPerBar: 3) { TempoData = new TempoData(120) { IsAgogic = true, IsPreviousTempoStartTempo = true, StartTempo = 75, EndTempo = 150} }},
                new ClickTrackSectionData (id: 2) { Title = "6/8 Beat", Length = 4, MetData = new MetronomeData(notesPerBar: 6) { NoteValue = 8, BeatAccents = [1, 0, 2, 0, 1, 0], TempoData = new TempoData(65) }}
            }
        },
        new ClickTrackData() {ID=2, Title="Inge Brauch - Beginnen zu Beginnen", CountIn=true, CountInBars=2,
            Sections = new List<ClickTrackSectionData>
            {
                new ClickTrackSectionData (id: 0) { Title = "Intro", Length = 10, MetData = new MetronomeData(notesPerBar: 6) { TempoData = new TempoData(220) }},
                new ClickTrackSectionData (id: 1) { Title = "Verse 1", Length = 7, MetData = new MetronomeData() { TempoData = new TempoData(137) }},
                new ClickTrackSectionData (id: 2) { Title = "Bridge", Length = 1, MetData = new MetronomeData(notesPerBar: 6) { TempoData = new TempoData(137) }},
                new ClickTrackSectionData (id: 3) { Title = "Verse 1 continued", Length = 7, MetData = new MetronomeData() { TempoData = new TempoData(137) }},
                new ClickTrackSectionData (id: 4) { Title = "Bridge", Length = 1, MetData = new MetronomeData(notesPerBar: 6) { TempoData = new TempoData(137) }},
                new ClickTrackSectionData (id: 5) { Title = "Pre-Chorus", Length = 8, MetData = new MetronomeData() { TempoData = new TempoData(137) { IsAgogic = true, StartTempo = 137, EndTempo = 157, IsPreviousTempoStartTempo = true, IsNextTempoEndTempo = true} }},
                new ClickTrackSectionData (id: 6) { Title = "Chorus", Length = 32, MetData = new MetronomeData() { TempoData = new TempoData(157) }},
                new ClickTrackSectionData (id: 7) { Title = "Interlude ", Length = 24, MetData = new MetronomeData(notesPerBar: 6) { TempoData = new TempoData(240) }}, // TODO: make this 12 bars 4/4 @ 120bpm with 6 subdivisions once subdivisions are added
                new ClickTrackSectionData (id: 8) { Title = "Verse 2", Length = 7, MetData = new MetronomeData() { TempoData = new TempoData(137) }},
                new ClickTrackSectionData (id: 9) { Title = "Bridge x3", Length = 3, MetData = new MetronomeData(notesPerBar: 6) { TempoData = new TempoData(137) { IsAgogic = true, StartTempo = 137, EndTempo = 157, IsPreviousTempoStartTempo = true, IsNextTempoEndTempo = true} }},
                new ClickTrackSectionData (id: 10) { Title = "Chorus", Length = 24, MetData = new MetronomeData() { TempoData = new TempoData(157) }},
                new ClickTrackSectionData (id: 11) { Title = "Outro ", Length = 64, MetData = new MetronomeData(notesPerBar: 6) { TempoData = new TempoData(240) }}, // TODO: the same as Interlude but 130bpm. Also use infinite for Length once added
            }
        },
    };

    private async Task<List<ClickTrackData>> EnsureLoaded()
    {
        _tracks ??= await storage.GetAsync<List<ClickTrackData>>(Key) ?? _defaultTracks;
        return _tracks;
    }

    public async Task<List<ClickTrackData>> GetAllAsync()
    {
        return await EnsureLoaded();
    }

    public async Task<ClickTrackData?> GetAsync(int id)
    {
        var tracks = await EnsureLoaded();
        return tracks.FirstOrDefault(t => t.ID == id);
    }

    public async Task RemoveAsync(int id)
    {
        var tracks = await EnsureLoaded();
        tracks.RemoveAll(t => t.ID == id);
        await storage.SetAsync(Key, tracks);
    }

    public async Task AddAsync()
    {
        var tracks = await EnsureLoaded();
        int newID = tracks.Max(s => s.ID) + 1;
        ClickTrackData newTrack = new ClickTrackData(){ID = newID};
        tracks.Add(newTrack);
        await SaveAllAsync(tracks);
    }

    public async Task UpdateAsync(ClickTrackData track)
    {
        var tracks = await EnsureLoaded();
        var index = tracks.FindIndex(t => t.ID == track.ID);
        if (index >= 0)
            tracks[index] = track;
        else
            tracks.Add(track);
        await SaveAllAsync(tracks);
    }

    public async Task SaveAllAsync(List<ClickTrackData> tracks)
    {
        _tracks = tracks;
        await storage.SetAsync(Key, tracks);
    }
}