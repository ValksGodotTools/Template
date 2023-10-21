namespace Template;

public partial class UIOptionsAudio : Control
{
    [Export] OptionsManager optionsManager;

    ResourceOptions options;

    public override void _Ready()
    {
        options = optionsManager.Options;
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
        Global.Services.Get<AudioManager>().SetMusicVolume(v);

    void _on_sounds_value_changed(float v) =>
        Global.Services.Get<AudioManager>().SetSFXVolume(v);
}
