using Godot;

namespace GodotUtils;

public static class ControlExtensions
{
    public static void CoverEntireRect(this Control control)
    {
        control.SetAnchorsAndOffsetsPreset(Control.LayoutPreset.FullRect);
    }

    public static void CenterToScreen(this Control control)
    {
        control.SetAnchorsAndOffsetsPreset(Control.LayoutPreset.Center);
    }
}

