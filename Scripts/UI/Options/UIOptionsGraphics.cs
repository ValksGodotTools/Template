namespace Template;

using Environment = Godot.Environment;

public partial class UIOptionsGraphics : Control
{
    [Export] OptionsManager optionsManager;

    ResourceOptions options;
    OptionButton antialiasing;

    public override void _Ready()
    {
        options = optionsManager.Options;
        SetupQualityPreset();
        SetupAntialiasing();
        //SetupWorldEnvironmentSettings();
    }

    void SetupWorldEnvironmentSettings()
    {
        HBoxContainer hbox = new HBoxContainer();
        hbox.AddChild(new Label
        {
            Text = "GLOW",
            CustomMinimumSize = new Vector2(200, 0)
        });
        CheckBox checkBox = new CheckBox
        {
            ButtonPressed = true
        };
        checkBox.Pressed += () =>
        {
            // SHOULD ONLY DO THIS WHEN WORLD ENVIRONMENT NODE IS IN SCENE
            /*WorldEnvironment worldEnvironment =
                Global.Services.Get<UIPopupMenu>().WorldEnvironment;

            if (worldEnvironment != null)
            {
                Environment environment = worldEnvironment.Environment;


            }*/
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
    }
}

public enum QualityPreset
{
    Low,
    Medium,
    High
}
