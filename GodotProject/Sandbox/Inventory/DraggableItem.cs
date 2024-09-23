using Godot;
using GodotUtils;

namespace Template.Inventory;

[Draggable]
public partial class DraggableItem : AnimatedSprite2D, IDraggable
{
    public void OnDragReleased()
    {
        Area2D area = CursorUtils2D.GetAreaUnder(this);

        DraggableItem otherItem = area?.GetParent<DraggableItem>();

        GD.Print(otherItem);
    }
}
