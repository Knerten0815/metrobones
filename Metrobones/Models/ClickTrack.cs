namespace Metrobones.Models;

public class ClickTrack
{
    public int ID { get; set; }
    public string Title {get; set;} = "New Click Track";
    public bool CountIn { get; set; } = false;
    public int CountInBars { get; set; } = 2;
    public List<ClickTrackSection> Sections { get; set; } = new List<ClickTrackSection>();
}
