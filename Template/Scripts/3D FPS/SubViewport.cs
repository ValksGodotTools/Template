namespace Template;

public partial class SubViewport : SubViewportContainer
{
    [Export] OptionsManager options;

    public override async void _Ready()
    {
        StretchShrink = options.Options.Resolution;

        // Need to wait one frame because there is no easy way that I know of to execute this
        // the below code after UIOptionsDisplay.cs gets instantiated
        await GU.WaitOneFrame(this);

        UIOptionsDisplay display = GetNode<UIPopupMenu>("%PopupMenu")
            .Options.GetNode<UIOptionsDisplay>("%Display");

        display.OnResolutionChanged += (resolution) =>
        {
            StretchShrink = options.Options.Resolution;
        };
    }
}
