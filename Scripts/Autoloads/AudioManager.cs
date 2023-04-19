namespace Template;

public partial class AudioManager : Node
{
    public static float MusicVolume { get; private set; }
    public static float SFXVolume   { get; private set; }

    private static AudioStreamPlayer MusicPlayer { get; set; }
    private static AudioStreamPlayer SFXPlayer   { get; set; }

    public static void PlayMusic(AudioStream song, bool instant = true, double fadeOut = 1.5, double fadeIn = 0.5)
    {
        if (!instant && MusicPlayer.Playing)
        {
            // Transition from current song being played to new song
            var tween = new GTween(MusicPlayer);
            tween.Create();

            // Fade out current song
            tween.Animate("volume_db", -80, fadeOut)
                .SetTrans(Tween.TransitionType.Sine)
                .SetEase(Tween.EaseType.In);

            // Set to new song
            tween.Callback(() =>
            {
                MusicPlayer.Stream = song;
                MusicPlayer.Play();
            });

            // Fade in to current song
            tween.Animate("volume_db", MusicVolume, fadeIn)
                .SetTrans(Tween.TransitionType.Sine)
                .SetEase(Tween.EaseType.In);
        }
        else
        {
            // Instantly switch to and play new song
            MusicPlayer.Stream = song;
            MusicPlayer.Play();
        }
    }

    // Remember, this is a temporary solution to SFX sounds
    // Imagine 100's of sounds being played at once. There is only
    // one SFX player. So each new sound will cancel out the previous.
    // To fix this, we need to create AudioStreamPlayers on demand.
    public static void PlaySFX(AudioStream sound)
    {
        SFXPlayer.Stream = sound;
        SFXPlayer.Play();
    }

    public static void SetMusicVolume(float v)
    {
        var remappedValue = v.Remap(0, 100, -40, 0);
        
        MusicVolume = remappedValue;
        MusicPlayer.VolumeDb = remappedValue <= -40 ? -80 : remappedValue;
    }

    public static void SetSFXVolume(float v)
    {
        var remappedValue = v.Remap(0, 100, -40, 0);
        
        SFXVolume = remappedValue;
        SFXPlayer.VolumeDb = remappedValue <= -40 ? -80 : remappedValue;
    }

    public override void _Ready()
    {
        MusicPlayer = GetNode<AudioStreamPlayer>("Music");
        SFXPlayer = GetNode<AudioStreamPlayer>("SFX");

        // TODO: Save 'MusicVolume' and 'SFXVolume' somewhere and then
        // load the values back in here

        PlayMusic(Music.Menu);
    }
}
