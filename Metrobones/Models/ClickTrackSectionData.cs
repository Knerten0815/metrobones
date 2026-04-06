using System.Text.Json.Serialization;
using System.Text.RegularExpressions;

namespace Metrobones.Models;

public class ClickTrackSectionData
{
    public int ID { get; set; }
    public bool IsOpen { get; set; } = true;
    public int Length {get; set;} = 4;
    public bool PlayForever { get; set; } = false;
    public MetronomeData MetData {get; set;} = new();
    public string Title {get; set;} = "Intro";
    [JsonIgnore]
    public Action<bool>? OnTrackAgogicsChanged { get; set;}

    public ClickTrackSectionData(){}

    public ClickTrackSectionData(int id)
    {
        ID = id;
    }

    public ClickTrackSectionData(ClickTrackSectionData data)
    {
        ID = data.ID;
        Title = IncrementTrailingNumber(data.Title);
        Length = data.Length;
        MetData = new MetronomeData(data.MetData);
    }

    public static string IncrementTrailingNumber(string input)
    {
        var match = Regex.Match(input, @"(\d+)$");

        if (match.Success)
        {
            int number = int.Parse(match.Value);
            return input[..match.Index] + (number + 1);
        }

        return input + " 2";
    }
}
