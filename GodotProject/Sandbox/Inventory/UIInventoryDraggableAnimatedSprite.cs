using Godot;
using GodotUtils;

namespace Template.Inventory;

[Draggable]
public partial class UIInventoryDraggableAnimatedSprite : AnimatedSprite2D, IDraggable
{
    public void OnDragReleased()
    {
        Node node = CursorUtils.GetAreaUnder(this);

        GD.Print(node?.GetParent().Name);
    }
}
