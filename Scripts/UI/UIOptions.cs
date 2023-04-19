namespace Template;

public partial class UIOptions : Node
{
    private VBoxContainer  VBox                { get; set; }
    private UIWindowSize   UIWindowSize        { get; set; }
    private UIOptionButton UIFullscreenOptions { get; set; }
    private UISlider       UIFPSSlider         { get; set; }

    public override void _Ready()
    {
        VBox = GetNode<VBoxContainer>("VBox");
        CreateMusicSlider();
        CreateSFXSlider();
        CreateFullscreenOptions();
        CreateVSync();
        CreateWindowSize();
        CreateMaxFPS();
        CreateLanguageOptions();

        // Set FPS to unlimited when any VSync mode is enabled
        if (Global.Options.VSyncMode != DisplayServer.VSyncMode.Disabled)
        {
            UIFPSSlider.Slider.Value = 0;
            UIFPSSlider.Slider.Editable = false;
        }

        Global.WindowModeChanged += windowMode =>
            UIFullscreenOptions.OptionButton.Select((int)windowMode);
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
            Name = "Sounds",
            HSlider = new HSlider
            {
                Value = Global.Options.SFXVolume
            }
        });

        slider.ValueChanged += v => AudioManager.SetSFXVolume(v);

        VBox.AddChild(slider);
    }

    private void CreateFullscreenOptions()
    {
        UIFullscreenOptions = new UIOptionButton(new OptionButtonOptions
        {
            Name = "Window Mode",
            Items = new string[]
            {
                "Windowed",
                "Borderless",
                "Fullscreen"
            }
        });

        UIFullscreenOptions.ValueChanged += v =>
        {
            switch ((WindowMode)v)
            {
                case WindowMode.Windowed:
                    DisplayServer.WindowSetMode(DisplayServer.WindowMode.Windowed);
                    Global.Options.WindowMode = WindowMode.Windowed;
                    break;
                case WindowMode.Borderless:
                    DisplayServer.WindowSetMode(DisplayServer.WindowMode.Fullscreen);
                    Global.Options.WindowMode = WindowMode.Borderless;
                    break;
                case WindowMode.Fullscreen:
                    DisplayServer.WindowSetMode(DisplayServer.WindowMode.ExclusiveFullscreen);
                    Global.Options.WindowMode = WindowMode.Fullscreen;
                    break;
            }

            // Update UIWindowSize element on window mode change
            var winSize = DisplayServer.WindowGetSize();

            UIWindowSize.ResX.Text = winSize.X + "";
            UIWindowSize.ResY.Text = winSize.Y + "";

            Global.Options.WindowSize = winSize;
        };

        VBox.AddChild(UIFullscreenOptions);

        UIFullscreenOptions.OptionButton.Select((int)Global.Options.WindowMode);
    }

    private void CreateVSync()
    {
        var options = new UIOptionButton(new OptionButtonOptions
        {
            Name = "VSync Mode",
            Items = new string[]
            {
                DisplayServer.VSyncMode.Disabled + "",
                DisplayServer.VSyncMode.Enabled + "",
                DisplayServer.VSyncMode.Adaptive + ""
            }
        });

        options.ValueChanged += v =>
        {
            var vsyncMode = (DisplayServer.VSyncMode)v;
            DisplayServer.WindowSetVsyncMode(vsyncMode);
            Global.Options.VSyncMode = vsyncMode;

            // Set FPS to unlimited when any VSync mode is enabled
            if (vsyncMode != DisplayServer.VSyncMode.Disabled)
            {
                UIFPSSlider.Slider.Value = 0;
                UIFPSSlider.Slider.Editable = false;
            }
            else
            {
                UIFPSSlider.Slider.Editable = true;
            }
        };

        VBox.AddChild(options);

        options.OptionButton.Select((int)Global.Options.VSyncMode);
    }

    private void CreateWindowSize()
    {
        UIWindowSize = new UIWindowSize(new ElementOptions
        {
            Name = "Window Size"
        });

        VBox.AddChild(UIWindowSize);
    }

    private string prevStr = "";

    private void CreateMaxFPS()
    {
        var hbox = new HBoxContainer();

        UIFPSSlider = new UISlider(new SliderOptions
        {
            Name = "Max FPS",
            HSlider = new HSlider
            {
                Value = Global.Options.MaxFPS,
                MinValue = 0,
                MaxValue = 120,
                AllowGreater = true
            }
        });

        var fpsLabel = new GLabel("");

        if (Global.Options.MaxFPS == 0)
            fpsLabel.Text = "Unlimited";

        UIFPSSlider.ValueChanged += v =>
        {
            if (v == 0)
                fpsLabel.Text = "Unlimited";
            else
                fpsLabel.Text = "";

            Engine.MaxFps = (int)v;
            Global.Options.MaxFPS = (int)v;
        };

        hbox.AddChild(UIFPSSlider);
        hbox.AddChild(fpsLabel);

        VBox.AddChild(hbox);
    }

    private void CreateLanguageOptions()
    {
        var options = new UIOptionButton(new OptionButtonOptions
        {
            Name = "Language",
            Items = new string[]
            {
                Language.English + "",
                Language.French + "",
                Language.Japanese + ""
            }
        });

        options.ValueChanged += v =>
        {
            switch ((Language)v)
            {
                case Language.English:
                    TranslationServer.SetLocale("en");
                    break;
                case Language.French:
                    TranslationServer.SetLocale("fr");
                    break;
                case Language.Japanese:
                    TranslationServer.SetLocale("ja");
                    break;
            }

            Global.Options.Language = (Language)v;
        };

        VBox.AddChild(options);

        options.OptionButton.Select((int)Global.Options.Language);
    }
}

public partial class UIWindowSize : UIElement
{
    public LineEdit ResX { get; set; }
    public LineEdit ResY { get; set; }

    private Vector2I WinSize { get; }

    private int _prevNumX;
    private int _prevNumY;

    public UIWindowSize(ElementOptions options) : base(options)
    {
        WinSize = DisplayServer.WindowGetSize();
        _prevNumX = WinSize.X;
        _prevNumY = WinSize.Y;
    }

    public override void CreateUI(HBoxContainer hbox)
    {
        var minWinSize = DisplayServer.WindowGetMinSize();
        var screenSize = DisplayServer.ScreenGetSize();

        ResX = new LineEdit
        {
            Text = WinSize.X + "",
            Alignment = HorizontalAlignment.Center
        };

        var padding = new Control
        {
            CustomMinimumSize = new Vector2(10, 0)
        };
        var labelX = new GLabel("x");

        ResY = new LineEdit
        {
            Text = WinSize.Y + "",
            Alignment = HorizontalAlignment.Center
        };

        var btn = new Button
        {
            Text = "Apply",
            CustomMinimumSize = new Vector2(100, 0)
        };

        ResX.TextChanged += text =>
        {
            Utils.ValidateNumber(text, ResX, 0, screenSize.X, ref _prevNumX);
        };

        ResY.TextChanged += text =>
        {
            Utils.ValidateNumber(text, ResY, 0, screenSize.Y, ref _prevNumY);
        };

        btn.Pressed += () =>
        {
            DisplayServer.WindowSetSize(new Vector2I(_prevNumX, _prevNumY));

            // Center window
            var winSize = DisplayServer.WindowGetSize();
            DisplayServer.WindowSetPosition(screenSize / 2 - winSize / 2);

            Global.Options.WindowSize = winSize;
        };

        hbox.AddChild(ResX);
        hbox.AddChild(padding);
        hbox.AddChild(labelX);
        hbox.AddChild(padding.Duplicate());
        hbox.AddChild(ResY);
        hbox.AddChild(padding.Duplicate());
        hbox.AddChild(btn);
    }
}

public enum Language
{
    English,
    French,
    Japanese
}

public enum WindowMode
{
    Windowed,
    Borderless,
    Fullscreen
}
