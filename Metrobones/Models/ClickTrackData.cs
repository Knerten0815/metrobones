namespace Metrobones.Models;

public class ClickTrackData
{
    public int ID { get; set; }
    public string Title {get; set;} = "New Click Track";
    public bool CountIn { get; set; } = false;
    public int CountInBars { get; set; } = 2;
    public List<ClickTrackSectionData> Sections { get; set; } = new List<ClickTrackSectionData>();
}
