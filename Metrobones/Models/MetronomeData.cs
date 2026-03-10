namespace Metrobones.Models;

public class MetronomeData
{
    public int Tempo { get; set; } = 120;
    public int NotesPerBar { get; set; } = 4;
    public int NoteValue { get; set; } = 4;
    public int[] BeatAccents { get; set; } = [1, 0, 0, 0];
}
