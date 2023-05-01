namespace Template;

public partial class AudioManager : Node
{                                                
    private static GAudioPlayer musicPlayer;
    private static Node sfxPlayersParent;
    private static float lastPitch;
    private static ResourceOptions options;

    public static void PlayMusic(AudioStream song, bool instant = true, double fadeOut = 1.5, double fadeIn = 0.5)
    {
        if (!instant && musicPlayer.Playing)
        {
            // Transition from current song being played to new song
            var tween = new GTween(musicPlayer.StreamPlayer);
            tween.Create();

            // Fade out current song
            tween.Animate("volume_db", -80, fadeOut)
                .SetTrans(Tween.TransitionType.Sine)
                .SetEase(Tween.EaseType.In);

            // Set to new song
            tween.Callback(() =>
            {
                musicPlayer.Stream = song;
                musicPlayer.Play();
            });

            // Fade in to current song
            var volume = options.MusicVolume;
            var volumeRemapped = volume == 0 ? -80 : volume.Remap(0, 100, -40, 0);
            tween.Animate("volume_db", volumeRemapped, fadeIn)
                .SetTrans(Tween.TransitionType.Sine)
                .SetEase(Tween.EaseType.In);
        }
        else
        {
            // Instantly switch to and play new song
            musicPlayer.Stream = song;
            musicPlayer.Volume = options.MusicVolume;
            musicPlayer.Play();
        }
    }

    public static void PlaySFX(AudioStream sound)
    {
        // Setup the SFX stream player
        var sfxPlayer = new GAudioPlayer(sfxPlayersParent, true)
        {
            Stream = sound,
            Volume = options.SFXVolume
        };

        // Randomize the pitch
        var rng = new RandomNumberGenerator();
        rng.Randomize();
        var pitch = rng.RandfRange(0.8f, 1.2f);

        // Ensure the current pitch is not the same as the last
        while (Mathf.Abs(pitch - lastPitch) < 0.1f)
        {
            rng.Randomize();
            pitch = rng.RandfRange(0.8f, 1.2f);
        }

        lastPitch = pitch;

        // Play the sound
        sfxPlayer.Pitch = pitch;
        sfxPlayer.Play();
    }

    /// <summary>
    /// Gradually fade out all sounds
    /// </summary>
    public static void FadeOutSFX(double fadeTime = 1)
    {
        foreach (AudioStreamPlayer audioPlayer in sfxPlayersParent.GetChildren())
        {
            var tween = new GTween(audioPlayer);
            tween.Create();
            tween.Animate("volume_db", -80, fadeTime);
        }
    }

    public static void SetMusicVolume(float v)
    {
        musicPlayer.Volume = v;
        options.MusicVolume = musicPlayer.Volume;
    }

    public static void SetSFXVolume(float v)
    {
        // Set volume for future SFX players
        options.SFXVolume = v;

        // Can't cast to GAudioPlayer so will have to remap manually again
        v = v == 0 ? -80 : v.Remap(0, 100, -40, 0);

        // Set volume of all SFX players currently in the scene
        foreach (AudioStreamPlayer audioPlayer in sfxPlayersParent.GetChildren())
            audioPlayer.VolumeDb = v;
    }

    public override void _Ready()
    {
        options = OptionsManager.Options;
        musicPlayer = new GAudioPlayer(this);

        sfxPlayersParent = new Node();
        AddChild(sfxPlayersParent);

        PlayMusic(Music.Menu);
    }
}
