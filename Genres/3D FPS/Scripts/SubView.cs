using Godot;
using Template.UI;

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

        OptionsDisplay display = popupMenu
            .Options.GetNode<OptionsDisplay>("%Display");

        display.OnResolutionChanged += (resolution) =>
        {
            StretchShrink = options.Options.Resolution;
        };

        OptionsGraphics graphics = popupMenu.Options.GetNode<OptionsGraphics>("%Graphics");

        graphics.OnAntialiasingChanged += (aa) =>
        {
            subViewport.Msaa3D = (Viewport.Msaa)aa;
        };
    }
}

