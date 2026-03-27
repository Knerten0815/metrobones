namespace Metrobones.Models;

public class TempoData
{
    public int Tempo { get; set; }
    public bool IsAgogic { get; set; } = false;
    public int PreviousTempo { get; set; } = -1;
    public int NextTempo { get; set; } = -1;
    public int StartTempo { get; set; } = -1;
    public int EndTempo { get; set;} = -1;

    public TempoData(int tempo = 120)
    {
        Tempo = tempo;
    }
}
