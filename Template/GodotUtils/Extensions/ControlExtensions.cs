using Godot;
using Layout = Godot.Control.LayoutPreset;

namespace GodotUtils;

public static class ControlExtensions
{
    /// <summary>
    /// Sets the layout for the specified control.
    /// </summary>
    /// <remarks>
    /// Applies the layout immediately if the control is inside the tree, otherwise applies it when the control becomes ready.
    /// </remarks>
    public static void SetLayout(this Control control, Layout layout)
    {
        if (control.IsInsideTree())
        {
            control.SetAnchorsAndOffsetsPreset(layout);
        }
        else
        {
            control.Ready += () => control.SetAnchorsAndOffsetsPreset(layout);
        }
    }
}
