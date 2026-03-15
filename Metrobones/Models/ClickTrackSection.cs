namespace Metrobones.Models;

public class ClickTrackSection
{
    public int ID { get; set; }
    public bool IsOpen { get; set; } = true;
    public int Length {get; set;} = 8;
    public MetronomeData MetData {get; set;} = new();

    public string Title {get; set;} = "Verse";

    public ClickTrackSection(int id)
    {
        ID = id;
    }
}
