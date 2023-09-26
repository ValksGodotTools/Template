namespace Template;

public partial class UIOptionsGraphics : Control
{
    [Export] OptionsManager optionsManager;

    ResourceOptions options;

    OptionButton optionBtnQualityPreset;

    public override void _Ready()
    {
        options = optionsManager.Options;
        SetupQualityPreset();
    }

    void SetupQualityPreset()
    {
        optionBtnQualityPreset = GetNode<OptionButton>("%QualityMode");
        optionBtnQualityPreset.Select((int)options.QualityPreset);
    }

    void _on_quality_mode_item_selected(int index)
    {
        // todo: setup quality preset and change other settings

        options.QualityPreset = (QualityPreset)index;
    }
}

public enum QualityPreset
{
    Low,
    Medium,
    High
}
