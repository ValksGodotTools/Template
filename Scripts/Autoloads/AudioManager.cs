namespace Template;

public partial class AudioManager : Node
{
    [Signal] 
    public delegate void VolumeMusicChangedEventHandler(float volume);

    [Signal]
    public delegate void VolumeSFXChangedEventHandler(float volume);

    public static AudioManager Instance { get; set; }

    private static Dictionary<string, float> Volume { get; } = new();

    public static void SetVolume(string name, float value)
    {
        var signalName = $"Volume{name}Changed";
        Volume[signalName] = value;
        var error = Instance.EmitSignal(signalName, value.Remap(0, 100, -40, 0));

        if (error == Error.Unavailable)
            Logger.LogWarning($"The volume signal '{signalName}' does not exist. " +
                $"Perhaps there was a typo in SetVolume({name}, {value})?");
    }

    private Dictionary<string, AudioStreamPlayer> StreamPlayers { get; } = new();

    public override void _Ready()
    {
        Instance = this;
        StreamPlayers["Music"] = GetNode<AudioStreamPlayer>("Music");
        StreamPlayers["SFX"] = GetNode<AudioStreamPlayer>("SFX");
    }
}
