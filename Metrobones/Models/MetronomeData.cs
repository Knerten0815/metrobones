namespace Metrobones.Models;

public class MetronomeData
{
    public int Tempo { get; set; } = 120;
    public int NotesPerBar { get; set; }
    public int NoteValue { get; set; } = 4;
    public int[] BeatAccents { get; set; }
    public double AgogicModifier { get; set; } = 1;

    public MetronomeData(int notesPerBar = 4)
    {
        NotesPerBar = notesPerBar;
        BeatAccents = new int[notesPerBar];
        BeatAccents[0] = 1;
    }
}
