namespace Template;

public partial class SubView : SubViewportContainer
{
    [Export] OptionsManager options;

    public override async void _Ready()
    {
        StretchShrink = options.Options.Resolution;

        SubViewport subViewport = GetNode<SubViewport>("SubViewport");
        subViewport.Msaa3D = (Viewport.Msaa)options.Options.Antialiasing;

        // Need to wait one frame because there is no easy way that I know of to execute this
        // the below code after UIOptionsDisplay.cs gets instantiated
        await this.WaitOneFrame();

        UIPopupMenu popupMenu = GetNode<UIPopupMenu>("%PopupMenu");

        UIOptionsDisplay display = popupMenu
            .Options.GetNode<UIOptionsDisplay>("%Display");

        display.OnResolutionChanged += (resolution) =>
        {
            StretchShrink = options.Options.Resolution;
        };

        UIOptionsGraphics graphics = popupMenu.Options.GetNode<UIOptionsGraphics>("%Graphics");

        graphics.OnAntialiasingChanged += (aa) =>
        {
            subViewport.Msaa3D = (Viewport.Msaa)aa;
        };
    }
}
