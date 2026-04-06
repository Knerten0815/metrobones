namespace Metrobones.Models;

public class TempoData
{
    public int Tempo { get; set; } = 120;
    public bool IsAgogic { get; set; } = false;
    public int PreviousTempo { get; set; } = -1;
    public int NextTempo { get; set; } = -1;
    public int StartTempo { get; set; } = -1;
    public int EndTempo { get; set;} = -1;
    public bool IsPreviousTempoStartTempo { get; set; } = false;
    public bool IsNextTempoEndTempo { get; set ; } = false;

    public TempoData(){}

    public TempoData(int tempo)
    {
        Tempo = tempo;
    }

    public TempoData(TempoData data)
    {
        Tempo = data.Tempo;
        IsAgogic = data.IsAgogic;
        PreviousTempo = data.PreviousTempo;
        NextTempo = data.NextTempo;
        StartTempo = data.StartTempo;
        EndTempo = data.EndTempo;
        IsPreviousTempoStartTempo = data.IsPreviousTempoStartTempo;
        IsNextTempoEndTempo = data.IsNextTempoEndTempo;
    }

    public void UpdateAgogics(bool hasPreviousSection = false, bool hasNextSection = false)
    {
        if (StartTempo <= 0 || (IsPreviousTempoStartTempo && hasPreviousSection == false))
        {
            StartTempo = PreviousTempo > 0 ? PreviousTempo : Tempo;
        }

        if (EndTempo <= 0 || (IsNextTempoEndTempo && hasNextSection == false))
        {
            EndTempo = NextTempo > 0 ? NextTempo : Tempo;
        }
    }
}
