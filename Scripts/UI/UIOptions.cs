namespace Template;

public partial class UIOptions : Control
{
    private VBoxContainer   TabGeneral          { get; set; }
    private VBoxContainer   TabHotkeys          { get; set; }

    private UIWindowSize    UIWindowSize        { get; set; }
    private UIOptionButton  UIFullscreenOptions { get; set; }
    private UISlider        UIFPSSlider         { get; set; }
    private ResourceOptions Options             { get; set; }

    public override void _Ready()
    {
        Options = OptionsManager.Options;

        var tabContainer = GetNode<TabContainer>("Center/Margin/Tab");

        TabGeneral = tabContainer.GetNode<VBoxContainer>("General");
        TabHotkeys = tabContainer.GetNode<VBoxContainer>("Hotkeys/VBox1/VBox2");

        // General
        CreateSliderMusic();
        CreateSliderSounds();
        CreateDropdownFullscreenMode();
        CreateDropdownVSyncMode();
        CreateLineEditWindowSize();
        CreateSliderMaxFPS();
        CreateDropdownLanguage();

        // Hotkeys
        CreateHotkeys();

        // Set FPS to unlimited when any VSync mode is enabled
        if (Options.VSyncMode != DisplayServer.VSyncMode.Disabled)
        {
            UIFPSSlider.Slider.Value = 0;
            UIFPSSlider.Slider.Editable = false;
        }

        OptionsManager.WindowModeChanged += windowMode =>
            UIFullscreenOptions.OptionButton.Select((int)windowMode);
    }

    private void CreateHotkeys() => TabHotkeys.AddChild(new UIHotkeys());

    private void CreateSliderMusic() =>
        CreateAudioSlider("Music", Options.MusicVolume, v =>
            AudioManager.SetMusicVolume(v));

    private void CreateSliderSounds() =>
        CreateAudioSlider("Sounds", Options.SFXVolume, v =>
            AudioManager.SetSFXVolume(v));

    private void CreateDropdownFullscreenMode()
    {
        UIFullscreenOptions = CreateOptionButton("Window Mode", new string[]
        {
            "Windowed",
            "Borderless",
            "Fullscreen"
        }, v =>
        {
            switch ((WindowMode)v)
            {
                case WindowMode.Windowed:
                    DisplayServer.WindowSetMode(DisplayServer.WindowMode.Windowed);
                    Options.WindowMode = WindowMode.Windowed;
                    break;
                case WindowMode.Borderless:
                    DisplayServer.WindowSetMode(DisplayServer.WindowMode.Fullscreen);
                    Options.WindowMode = WindowMode.Borderless;
                    break;
                case WindowMode.Fullscreen:
                    DisplayServer.WindowSetMode(DisplayServer.WindowMode.ExclusiveFullscreen);
                    Options.WindowMode = WindowMode.Fullscreen;
                    break;
            }

            // Update UIWindowSize element on window mode change
            var winSize = DisplayServer.WindowGetSize();

            UIWindowSize.ResX.Text = winSize.X + "";
            UIWindowSize.ResY.Text = winSize.Y + "";
            UIWindowSize.PrevNumX = winSize.X;
            UIWindowSize.PrevNumY = winSize.Y;

            Options.WindowSize = winSize;
        }, (int)Options.WindowMode);
    }

    private void CreateDropdownVSyncMode()
    {
        CreateOptionButton("VSync Mode", new string[]
        {
            DisplayServer.VSyncMode.Disabled + "",
            DisplayServer.VSyncMode.Enabled + "",
            DisplayServer.VSyncMode.Adaptive + ""
        }, v =>
        {
            var vsyncMode = (DisplayServer.VSyncMode)v;
            DisplayServer.WindowSetVsyncMode(vsyncMode);
            Options.VSyncMode = vsyncMode;

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
        }, (int)Options.VSyncMode);
    }

    private void CreateLineEditWindowSize()
    {
        UIWindowSize = new UIWindowSize(new ElementOptions
        {
            Name = "Window Size"
        });

        TabGeneral.AddChild(UIWindowSize);
    }

    private void CreateSliderMaxFPS()
    {
        var hbox = new HBoxContainer();

        UIFPSSlider = new UISlider(new SliderOptions
        {
            Name = "Max FPS",
            HSlider = new HSlider
            {
                Value = Options.MaxFPS,
                MinValue = 0,
                MaxValue = 120,
                AllowGreater = true
            }
        });

        var fpsLabel = new GLabel("");

        if (Options.MaxFPS == 0)
            fpsLabel.Text = "Unlimited";

        UIFPSSlider.ValueChanged += v =>
        {
            if (v == 0)
                fpsLabel.Text = "Unlimited";
            else
                fpsLabel.Text = "";

            Engine.MaxFps = (int)v;
            Options.MaxFPS = (int)v;
        };

        hbox.AddChild(UIFPSSlider);
        hbox.AddChild(fpsLabel);

        TabGeneral.AddChild(hbox);
    }

    private void CreateDropdownLanguage()
    {
        CreateOptionButton("Language", new string[]
        {
            Language.English + "",
            Language.French + "",
            Language.Japanese + ""
        }, v =>
        {
            var locale = ((Language)v).ToString().Substring(0, 2).ToLower();

            TranslationServer.SetLocale(locale);

            Options.Language = (Language)v;
        }, (int)Options.Language);
    }

    private void CreateAudioSlider(string name, double initialValue, Action<float> valueChanged)
    {
        var slider = new UISlider(new SliderOptions
        {
            Name = name,
            HSlider = new HSlider
            {
                Value = initialValue
            }
        });

        slider.ValueChanged += v => valueChanged(v);

        TabGeneral.AddChild(slider);
    }

    private UIOptionButton CreateOptionButton(string name, string[] items, Action<long> valueChanged, int selected)
    {
        var optionBtn = new UIOptionButton(new OptionButtonOptions
        {
            Name = name,
            Items = items
        });

        optionBtn.ValueChanged += v => valueChanged(v);

        TabGeneral.AddChild(optionBtn);

        optionBtn.OptionButton.Select(selected);

        return optionBtn;
    }
}

public partial class UIWindowSize : UIElement
{
    public LineEdit ResX { get; set; }
    public LineEdit ResY { get; set; }

    public int PrevNumX;
    public int PrevNumY;

    private Vector2I WinSize { get; }

    public UIWindowSize(ElementOptions options) : base(options)
    {
        WinSize = DisplayServer.WindowGetSize();
        PrevNumX = WinSize.X;
        PrevNumY = WinSize.Y;
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
            Utils.ValidateNumber(text, ResX, 0, screenSize.X, ref PrevNumX);
        };

        ResY.TextChanged += text =>
        {
            Utils.ValidateNumber(text, ResY, 0, screenSize.Y, ref PrevNumY);
        };

        btn.Pressed += () =>
        {
            DisplayServer.WindowSetSize(new Vector2I(PrevNumX, PrevNumY));

            // Center window
            var winSize = DisplayServer.WindowGetSize();
            DisplayServer.WindowSetPosition(screenSize / 2 - winSize / 2);

            OptionsManager.Options.WindowSize = winSize;
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
