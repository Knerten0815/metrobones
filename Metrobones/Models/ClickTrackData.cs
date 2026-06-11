namespace Metrobones.Models;

public class ClickTrackData
{
    public int ID { get; set; }
    public string Title {get; set;} = "New Click Track";
    public bool CountIn { get; set; } = false;
    public int CountInBars { get; set; } = 2;
    public List<ClickTrackSectionData> Sections { get; set; } = new List<ClickTrackSectionData>();

    public ClickTrackData(){}
    public ClickTrackData(ClickTrackData data, int id)
    {
        ID = id;
        Title = data.Title;
        CountIn = data.CountIn;
        CountInBars = data.CountInBars;
        foreach(ClickTrackSectionData section in data.Sections)
        {
            Sections.Add(new ClickTrackSectionData(section));
        }
    }
}
