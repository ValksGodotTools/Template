namespace Template;

public partial class UIOptionsAudio : Control
{
    private ResourceOptions options;

    public override void _Ready()
    {
        options = OptionsManager.Options;
        SetupMusic();
        SetupSounds();
    }

    private void SetupMusic()
    {
        var slider = GetNode<HSlider>("Music/Music");
        slider.Value = options.MusicVolume;
    }

    private void SetupSounds()
    {
        var slider = GetNode<HSlider>("Sounds/Sounds");
        slider.Value = options.SFXVolume;
    }

    private void _on_music_value_changed(float v) =>
        AudioManager.SetMusicVolume(v);

    private void _on_sounds_value_changed(float v) =>
        AudioManager.SetSFXVolume(v);
}
