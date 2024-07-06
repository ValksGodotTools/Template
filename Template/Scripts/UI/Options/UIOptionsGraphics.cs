namespace Template;

using Environment = Godot.Environment;

public partial class UIOptionsGraphics : Control
{
    public event Action<int> OnAntialiasingChanged;

    [Export] OptionsManager optionsManager;

    ResourceOptions options;
    OptionButton antialiasing;

    public override void _Ready()
    {
        options = optionsManager.Options;
        SetupQualityPreset();
        SetupAntialiasing();
        SetupWorldEnvironmentSettings();
    }

    void SetupWorldEnvironmentSettings()
    {
        AddNewSetting("GLOW", 
            (checkbox) =>
                checkbox.ButtonPressed = options.Glow,
            (pressed) =>
                options.Glow = pressed, 
            (environment, pressed) =>
                environment.GlowEnabled = pressed);

        AddNewSetting("AMBIENT_OCCLUSION",
            (checkbox) =>
                checkbox.ButtonPressed = options.AmbientOcclusion,
            (pressed) =>
                options.AmbientOcclusion = pressed,
            (environment, pressed) =>
                environment.SsaoEnabled = pressed);

        AddNewSetting("INDIRECT_LIGHTING",
            (checkbox) =>
                checkbox.ButtonPressed = options.IndirectLighting,
            (pressed) =>
                options.IndirectLighting = pressed,
            (environment, pressed) =>
                environment.SsilEnabled = pressed);

        AddNewSetting("REFLECTIONS",
            (checkbox) =>
                checkbox.ButtonPressed = options.Reflections,
            (pressed) =>
                options.Reflections = pressed,
            (environment, pressed) =>
                environment.SsrEnabled = pressed);
    }

    void AddNewSetting(string name, Action<CheckBox> setPressed, Action<bool> saveOption, Action<Environment, bool> applyInGame)
    {
        HBoxContainer hbox = new HBoxContainer();

        hbox.AddChild(new Label
        {
            Text = name,
            CustomMinimumSize = new Vector2(200, 0)
        });

        CheckBox checkBox = new CheckBox();
        setPressed(checkBox);

        checkBox.Pressed += () =>
        {
            saveOption(checkBox.ButtonPressed);

            UIPopupMenu popupMenu = Global.Services.Get<UIPopupMenu>();

            if (popupMenu == null)
                return;

            WorldEnvironment worldEnvironment = popupMenu.WorldEnvironment;

            if (worldEnvironment == null)
                return;

            applyInGame(worldEnvironment.Environment, checkBox.ButtonPressed);
        };

        hbox.AddChild(checkBox);

        AddChild(hbox);
    }

    void SetupQualityPreset()
    {
        OptionButton optionBtnQualityPreset = GetNode<OptionButton>("%QualityMode");
        optionBtnQualityPreset.Select((int)options.QualityPreset);
    }

    void SetupAntialiasing()
    {
        antialiasing = GetNode<OptionButton>("%Antialiasing");
        antialiasing.Select(options.Antialiasing);
    }

    void _on_quality_mode_item_selected(int index)
    {
        // todo: setup quality preset and change other settings

        options.QualityPreset = (QualityPreset)index;
    }

    void _on_antialiasing_item_selected(int index)
    {
        options.Antialiasing = index;
        OnAntialiasingChanged?.Invoke(index);
    }
}

public enum QualityPreset
{
    Low,
    Medium,
    High
}
