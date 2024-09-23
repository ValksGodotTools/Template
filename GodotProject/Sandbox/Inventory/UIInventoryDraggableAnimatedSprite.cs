using Godot;
using GodotUtils;

namespace Template.Inventory;

[Draggable]
public partial class UIInventoryDraggableAnimatedSprite : AnimatedSprite2D, IDraggable
{
    public void OnDragReleased()
    {
        Area2D area = CursorUtils2D.GetAreaUnder(this);

        GD.Print(area?.GetParent().GetType());
    }
}
