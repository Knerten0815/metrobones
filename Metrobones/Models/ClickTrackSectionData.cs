namespace Metrobones.Models;

public class ClickTrackSectionData
{
    public int ID { get; set; }
    public bool IsOpen { get; set; } = true;
    public int Length {get; set;} = 8;
    public MetronomeData MetData {get; set;} = new();

    public string Title {get; set;} = "Verse";

    public ClickTrackSectionData(int id)
    {
        ID = id;
    }
}
