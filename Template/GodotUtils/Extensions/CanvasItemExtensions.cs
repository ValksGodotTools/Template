using Godot;

namespace GodotUtils;

public static class CanvasItemExtensions
{
    public static void SetUnshaded(this CanvasItem canvasItem)
    {
        canvasItem.Material = new CanvasItemMaterial
        {
            LightMode = CanvasItemMaterial.LightModeEnum.Unshaded
        };
    }

    /// <summary>
    /// <para>Convert the CanvasItem's local position to a screen position</para>
    /// 
    /// <para>Using Camera2D.GetScreenCenterPosition() as a reference may prove useful</para>
    /// </summary>
    public static Vector2 GetScreenPosition(this CanvasItem canvasItem)
    {
        // Code retrieved from https://www.reddit.com/r/godot/comments/1aq1f0b/comment/kqa6z0u/
        // Relevant Godot Docs at https://docs.godotengine.org/en/stable/tutorials/2d/2d_transforms.html#transform-functions
        Window root = canvasItem.GetTree().Root;

        return (root.GetFinalTransform() * canvasItem.GetGlobalTransformWithCanvas()).Origin 
            + root.Position;
    }
}

