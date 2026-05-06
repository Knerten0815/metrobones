using Metrobones.Models;

namespace Metrobones.Services;

public class SoundManager(LocalStorage storage, ILogger<SoundManager> logger)
{
    private const string SoundSettingsKey = "soundSettings";
    private bool _initialized;
    private SoundData? _defaults = null;
    private SoundData Defaults
    {
        get
        {
            if(_defaults == null)
            {
                logger.LogError("SoundManager was not initialized! Using default sound settings.");
                _defaults = new SoundData();
            }
            return _defaults;
        }
    }

    public event Action? OnSoundSettingsChanged;

    public string Waveform
    {
        get => Defaults.Waveform;
        set
        {
            Defaults.Waveform = value;
            _ = Persist();
        }
    }

    public double Volume
    {
        get => Defaults.Volume;
        set
        {
            Defaults.Volume = value;
            _ = Persist();
        }
    }

    public double OnBeatPitch
    {
        get => Defaults.OnBeatPitch;
        set
        {
            Defaults.OnBeatPitch = value;
            _ = Persist();
        }
    }

    public double OffBeatPitch
    {
        get => Defaults.OffBeatPitch;
        set
        {
            Defaults.OffBeatPitch = value;
            _ = Persist();
        }
    }

    public async Task InitializeAsync()
    {
        if (_initialized)
            return;

        _initialized = true;

        SoundData? settings = await storage.GetAsync<SoundData>(SoundSettingsKey);
        if (settings != null)
            _defaults = settings;
    }

    public SoundData GetDefaultsSnapshot()
    {
        return new SoundData(Defaults);
    }

    public async Task ResetToDefaults()
    {
        _defaults = new SoundData();
        await Persist();
    }

    private async Task Persist()
    {
        OnSoundSettingsChanged?.Invoke();
        await storage.SetAsync(SoundSettingsKey, Defaults);
    }
}