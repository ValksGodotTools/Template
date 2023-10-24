namespace Template;

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
