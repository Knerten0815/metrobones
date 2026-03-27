namespace Metrobones.Models;

public class MetronomeData
{
    public int NotesPerBar { get; set; }
    public int NoteValue { get; set; } = 4;
    public int[] BeatAccents { get; set; }
    public TempoData TempoData{ get; set; }

    public MetronomeData(int notesPerBar = 4)
    {
        NotesPerBar = notesPerBar;
        BeatAccents = new int[notesPerBar];
        BeatAccents[0] = 1;
        TempoData = new TempoData();
    }
}
