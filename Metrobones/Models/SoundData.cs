namespace Metrobones.Models;

public class SoundData
{
    public string Waveform { get; set; } = "sine";
    public double Volume { get; set; } = 100;
    public double OnBeatPitch { get; set; } = 1500;
    public double OffBeatPitch { get; set; } = 1000;

    public SoundData() {}

    public SoundData(SoundData settings)
    {
        Waveform = settings.Waveform;
        Volume = settings.Volume;
        OnBeatPitch = settings.OnBeatPitch;
        OffBeatPitch = settings.OffBeatPitch;
    }
}