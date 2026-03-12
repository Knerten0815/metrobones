namespace Metrobones.Models;

public class ClickTrack
{
    public int ID { get; set; }
    public string Title {get; set;} = "Click Track";
    public bool CountIn { get; set; } = false;
    public int CountInBars { get; set; } = 2;
    public List<ClickTrackSection> Sections { get; set; } = new List<ClickTrackSection>
        {
            new ClickTrackSection (id: 1) { Title = "Intro", Length = 1, MetData = new MetronomeData(notesPerBar: 3) { Tempo = 150 }},
            new ClickTrackSection (id: 2) { Title = "Verse", Length = 2 , MetData = new MetronomeData(notesPerBar: 4) { Tempo = 120 }},
            new ClickTrackSection (id: 3) { Title = "Chorus", Length = 2, MetData = new MetronomeData() { Tempo = 150 }}
        };
}
