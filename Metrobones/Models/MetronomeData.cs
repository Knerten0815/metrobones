namespace Metrobones.Models;

public class MetronomeData
{
    public int NotesPerBar { get; set; } = 4;
    public int NoteValue { get; set; } = 4;
    public int Subdivisions { get; set; } = -1;
    public int[] BeatAccents { get; set; } = { 1, 0, 0, 0 };
    public TempoData TempoData{ get; set; } = new TempoData();

    public MetronomeData(){}

    public MetronomeData(int notesPerBar)
    {
        NotesPerBar = notesPerBar;
        BeatAccents = new int[notesPerBar];
        BeatAccents[0] = 1;
    }

    public MetronomeData(int notesPerBar, int noteValue, int subdivisions=-1)
    {
        NotesPerBar = notesPerBar;
        NoteValue = noteValue;
        Subdivisions = subdivisions;
        if(subdivisions > 0)
            BeatAccents = new int[subdivisions];
        else
            BeatAccents = new int[notesPerBar];
        BeatAccents[0] = 1;
    }

    public MetronomeData(MetronomeData data)
    {
        NotesPerBar = data.NotesPerBar;
        NoteValue = data.NoteValue;
        Subdivisions = data.Subdivisions;
        BeatAccents = data.BeatAccents;
        TempoData = new TempoData(data.TempoData);
    }
}
