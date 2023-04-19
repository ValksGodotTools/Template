namespace Template;

public partial class AudioManager : Node
{
    public  static float        MusicVolume      { get; private set; } = 100;
    public  static float        SFXVolume        { get; private set; } = 100;
                                                 
    private static GAudioPlayer MusicPlayer      { get; set; }

    private static Node         SFXPlayersParent { get; set; }
    private static float        LastPitch        { get; set; }

    public static void PlayMusic(AudioStream song, bool instant = true, double fadeOut = 1.5, double fadeIn = 0.5)
    {
        if (!instant && MusicPlayer.Playing)
        {
            // Transition from current song being played to new song
            var tween = new GTween(MusicPlayer.StreamPlayer);
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
            var volume = MusicVolume == 0 ? -80 : MusicVolume.Remap(0, 100, -40, 0);
            tween.Animate("volume_db", volume, fadeIn)
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

    public static void PlaySFX(AudioStream sound)
    {
        // Setup the SFX stream player
        var sfxPlayer = new GAudioPlayer(SFXPlayersParent, true)
        {
            Stream = sound,
            Volume = SFXVolume
        };

        // Randomize the pitch
        var rng = new RandomNumberGenerator();
        rng.Randomize();
        var pitch = rng.RandfRange(0.8f, 1.2f);

        // Ensure the current pitch is not the same as the last
        while (Mathf.Abs(pitch - LastPitch) < 0.1f)
        {
            rng.Randomize();
            pitch = rng.RandfRange(0.8f, 1.2f);
        }

        LastPitch = pitch;

        // Play the sound
        sfxPlayer.Pitch = pitch;
        sfxPlayer.Play();
    }

    /// <summary>
    /// Gradually fade out all sounds
    /// </summary>
    public static void FadeOutSFX(double fadeTime = 1)
    {
        foreach (AudioStreamPlayer audioPlayer in SFXPlayersParent.GetChildren())
        {
            var tween = new GTween(audioPlayer);
            tween.Create();
            tween.Animate("volume_db", -80, fadeTime);
        }
    }

    public static void SetMusicVolume(float v)
    {
        MusicPlayer.Volume = v;
        MusicVolume = MusicPlayer.Volume;
    }

    public static void SetSFXVolume(float v)
    {
        // Set volume for future SFX players
        SFXVolume = v;

        // Can't cast to GAudioPlayer so will have to remap manually again
        v = v == 0 ? -80 : v.Remap(0, 100, -40, 0);

        // Set volume of all SFX players currently in the scene
        foreach (AudioStreamPlayer audioPlayer in SFXPlayersParent.GetChildren())
            audioPlayer.VolumeDb = v;
    }

    public override void _Ready()
    {
        MusicPlayer = new GAudioPlayer(this);

        SFXPlayersParent = new Node();
        AddChild(SFXPlayersParent);

        // TODO: Save 'MusicVolume' and 'SFXVolume' somewhere and then
        // load the values back in here

        PlayMusic(Music.Menu);
    }
}
