namespace Template;

public partial class UIOptions : Node
{
    private VBoxContainer VBox { get; set; }

    public override void _Ready()
    {
        VBox = GetNode<VBoxContainer>("VBox");
        CreateMusicSlider();
        CreateSFXSlider();
    }

    public override void _Input(InputEvent @event)
    {
        if (Input.IsActionJustPressed("ui_cancel"))
        {
            AudioManager.PlayMusic(Music.Menu);
            SceneManager.SwitchScene("main_menu");
        }
    }

    private void CreateMusicSlider()
    {
        var slider = new UISlider(new SliderOptions
        {
            Name = "Music",
            HSlider = new HSlider
            {
                Value = Global.Options.MusicVolume
            }
        });

        slider.ValueChanged += v => AudioManager.SetMusicVolume(v);

        VBox.AddChild(slider);
    }

    private void CreateSFXSlider()
    {
        var slider = new UISlider(new SliderOptions
        {
            Name = "SFX",
            HSlider = new HSlider
            {
                Value = Global.Options.SFXVolume
            }
        });

        slider.ValueChanged += v => AudioManager.SetSFXVolume(v);

        VBox.AddChild(slider);
    }
}
