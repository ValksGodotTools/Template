namespace Template;

public partial class SubViewport : SubViewportContainer
{
    [Export] OptionsManager options;

    public override void _Ready()
    {
        StretchShrink = options.Options.Resolution;

        // No idea how to get this working...
        /*Global.Services.Get<UIOptionsDisplay>().OnResolutionChanged += (resolution) =>
        {
            StretchShrink = options.Options.Resolution;
        };*/
    }
}
