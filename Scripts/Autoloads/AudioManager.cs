namespace Template;

public partial class AudioManager : Node
{
    [Signal] 
    public delegate void VolumeMusicChangedEventHandler(float volume);

    [Signal]
    public delegate void VolumeSFXChangedEventHandler(float volume);

    public static AudioManager Instance { get; set; }

    public static float VolumeMusic
    {
        get => _volumeMusic;
        set
        {
            _volumeMusic = value;
            EmitRemappedValue(SignalName.VolumeMusicChanged, value);
        }
    }

    public static float VolumeSFX
    {
        get => _volumeSFX;
        set
        {
            _volumeSFX = value;
            EmitRemappedValue(SignalName.VolumeSFXChanged, value);
        }
    }

    private static float _volumeMusic;
    private static float _volumeSFX;

    private static void EmitRemappedValue(string signalName, float value) =>
        Instance.EmitSignal(signalName, value.Remap(0, 100, -40, 0));

    public override void _Ready() => Instance = this;
}
