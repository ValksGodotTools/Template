using Godot;

namespace Template;

public partial class UIOptionsAudio : Control
{
    [Export] OptionsManager optionsManager;

    ResourceOptions _options;

    public override void _Ready()
    {
        _options = optionsManager.Options;
        SetupMusic();
        SetupSounds();
    }

    void SetupMusic()
    {
        HSlider slider = GetNode<HSlider>("%Music");
        slider.Value = _options.MusicVolume;
    }

    void SetupSounds()
    {
        HSlider slider = GetNode<HSlider>("%Sounds");
        slider.Value = _options.SFXVolume;
    }

    void _on_music_value_changed(float v)
    {
        Global.Services.Get<AudioManager>().SetMusicVolume(v);
    }

    void _on_sounds_value_changed(float v)
    {
        Global.Services.Get<AudioManager>().SetSFXVolume(v);
    }
}

