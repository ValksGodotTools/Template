namespace GodotUtils;

using Godot;

public static class ExtensionsControl
{
    public static void CoverEntireRect(this Control control) =>
        control.SetAnchorsAndOffsetsPreset(Control.LayoutPreset.FullRect);

    public static void CenterToScreen(this Control control) =>
        control.SetAnchorsAndOffsetsPreset(Control.LayoutPreset.Center);
}
