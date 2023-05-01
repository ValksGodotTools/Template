namespace Template;

public partial class UIOptionsGraphics : Control
{
    private ResourceOptions options;

    private OptionButton optionBtnQualityPreset;

    public override void _Ready()
    {
        options = OptionsManager.Options;
        SetupQualityPreset();
    }

    private void SetupQualityPreset()
    {
        optionBtnQualityPreset = GetNode<OptionButton>("QualityMode/QualityMode");
        optionBtnQualityPreset.Select((int)options.QualityPreset);
    }

    private void _on_quality_mode_item_selected(int index)
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
