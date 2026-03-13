using Metrobones.Models;

namespace Metrobones.Services;

public class ClickTrackStore(LocalStorage storage)
{
    private const string Key = "clicktracks";
    private List<ClickTrack>? _tracks;
    private List<ClickTrack> _defaultTracks { get; set; } = new List<ClickTrack>()
    {
        new ClickTrack() {ID=0, Title="Simple Song", Sections = new List<ClickTrackSection>
            {
                new ClickTrackSection (id: 0) { Title = "Intro", Length = 2, MetData = new MetronomeData(){Tempo=100}},
                new ClickTrackSection (id: 1) { Title = "Verse", Length = 4 , MetData = new MetronomeData()},
            }
        },
        new ClickTrack() {ID=1, Title="Inge Brauch - Beginnen zu Beginnen", CountIn=true, CountInBars=2,
            Sections = new List<ClickTrackSection>
            {
                new ClickTrackSection (id: 0) { Title = "Intro", Length = 16, MetData = new MetronomeData(notesPerBar: 6) { NoteValue = 8, Tempo = 120 }},
                new ClickTrackSection (id: 1) { Title = "Verse 1", Length = 8, MetData = new MetronomeData() { Tempo = 120 }},
                new ClickTrackSection (id: 2) { Title = "Pre-Chorus", Length = 8, MetData = new MetronomeData() { Tempo = 120 }}
            }
        },
        new ClickTrack() {ID=2, Title="Weird Track", 
            Sections = new List<ClickTrackSection>
            {
                new ClickTrackSection (id: 0) { Title = "Intro", Length = 4, MetData = new MetronomeData(notesPerBar: 3) { Tempo = 80 }},
                new ClickTrackSection (id: 1) { Title = "Chorus", Length = 4, MetData = new MetronomeData(notesPerBar: 5) { Tempo = 160 }},
                new ClickTrackSection (id: 2) { Title = "Verse", Length = 4, MetData = new MetronomeData() { Tempo = 80 }}
            }
        }
    };

    private async Task<List<ClickTrack>> EnsureLoaded()
    {
        _tracks ??= await storage.GetAsync<List<ClickTrack>>(Key) ?? _defaultTracks;
        return _tracks;
    }

    public async Task<List<ClickTrack>> GetAllAsync()
        => await EnsureLoaded();

    public async Task<ClickTrack?> GetAsync(int id)
        => (await EnsureLoaded()).FirstOrDefault(t => t.ID == id);

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
        ClickTrack newTrack = new ClickTrack(){ID = newID};
        tracks.Add(newTrack);
        await SaveAllAsync(tracks);
    }

    public async Task UpdateAsync(ClickTrack track)
    {
        var tracks = await EnsureLoaded();
        var index = tracks.FindIndex(t => t.ID == track.ID);
        if (index >= 0)
            tracks[index] = track;
        else
            tracks.Add(track);
        await SaveAllAsync(tracks);
    }

    public async Task SaveAllAsync(List<ClickTrack> tracks)
    {
        _tracks = tracks;
        await storage.SetAsync(Key, tracks);
    }
}