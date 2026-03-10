namespace Metrobones.Models;

public class ClickTrackSection
{
    public int ID { get; set; }
    public int Length {get; set;} = 8;
    public MetronomeData MetData {get; set;} = new();

    public string Title {get; set;} = "Verse";
    public bool Open {get; set;} = false;

    public ClickTrackSection(int id)
    {
        ID = id;
    }
}
