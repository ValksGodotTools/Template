namespace Template;

public partial class UIOptionsAudio : Control
{
    private ResourceOptions Options { get; set; }

    public override void _Ready()
    {
        Options = OptionsManager.Options;

        SetupMusic();
        SetupSounds();
    }

    private void SetupMusic()
    {
        var slider = GetNode<HSlider>("Music/Music");
        slider.Value = Options.MusicVolume;
    }

    private void SetupSounds()
    {
        var slider = GetNode<HSlider>("Sounds/Sounds");
        slider.Value = Options.SFXVolume;
    }

    private void _on_music_value_changed(float v) =>
        AudioManager.SetMusicVolume(v);

    private void _on_sounds_value_changed(float v) =>
        AudioManager.SetSFXVolume(v);
}
