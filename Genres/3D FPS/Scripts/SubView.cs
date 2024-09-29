using Godot;

namespace Template.FPS3D;

public partial class SubView : SubViewportContainer
{
    [Export] private OptionsManager options;

    public override void _Ready()
    {
        StretchShrink = options.Options.Resolution;

        SubViewport subViewport = GetNode<SubViewport>("SubViewport");
        subViewport.Msaa3D = (Viewport.Msaa)options.Options.Antialiasing;

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

