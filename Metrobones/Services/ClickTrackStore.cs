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
                new ClickTrackSectionData (id: 1) { Title = "Verse", Length = 4 , MetData = new MetronomeData()},
            }
        },
        new ClickTrackData() {ID=1, Title="Inge Brauch - Beginnen zu Beginnen", CountIn=true, CountInBars=2,
            Sections = new List<ClickTrackSectionData>
            {
                new ClickTrackSectionData (id: 0) { Title = "Intro", Length = 16, MetData = new MetronomeData(notesPerBar: 6) { NoteValue = 8, TempoData = new TempoData(120) }},
                new ClickTrackSectionData (id: 1) { Title = "Verse 1", Length = 8, MetData = new MetronomeData() { TempoData = new TempoData(120) }},
                new ClickTrackSectionData (id: 2) { Title = "Pre-Chorus", Length = 8, MetData = new MetronomeData() { TempoData = new TempoData(120) }}
            }
        },
        new ClickTrackData() {ID=2, Title="Weird Track", 
            Sections = new List<ClickTrackSectionData>
            {
                new ClickTrackSectionData (id: 0) { Title = "Intro", Length = 4, MetData = new MetronomeData(notesPerBar: 3) { TempoData = new TempoData(80) }},
                new ClickTrackSectionData (id: 1) { Title = "Chorus", Length = 4, MetData = new MetronomeData(notesPerBar: 5) { TempoData = new TempoData(160) }},
                new ClickTrackSectionData (id: 2) { Title = "Verse", Length = 4, MetData = new MetronomeData() { TempoData = new TempoData(80) }}
            }
        }
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