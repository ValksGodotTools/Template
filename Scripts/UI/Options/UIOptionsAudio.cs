namespace Template;

public partial class UIOptionsAudio : Control
{
    ResourceOptions options;

    public override void _Ready()
    {
        options = OptionsManager.Options;
        SetupMusic();
        SetupSounds();
    }

    void SetupMusic()
    {
        var slider = GetNode<HSlider>("%Music");
        slider.Value = options.MusicVolume;
    }

    void SetupSounds()
    {
        var slider = GetNode<HSlider>("%Sounds");
        slider.Value = options.SFXVolume;
    }

    void _on_music_value_changed(float v) =>
        AudioManager.SetMusicVolume(v);

    void _on_sounds_value_changed(float v) =>
        AudioManager.SetSFXVolume(v);
}
