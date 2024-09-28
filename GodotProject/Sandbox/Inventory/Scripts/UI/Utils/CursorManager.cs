using Godot;

namespace Template.Inventory;

public class CursorManager(Inventory inv, CursorItemContainer cursorItemContainer) : InventoryManager(inv, 0, cursorItemContainer)
{
    public override void SetItemAndFrame(Item item, int frame)
    {
        base.SetItemAndFrame(item, frame);
        cursorItemContainer.SetPhysicsProcess(true);
    }

    public void SetPosition(Vector2 position)
    {
        cursorItemContainer.SetPosition(position);
        cursorItemContainer.ResetSmoothFactor();
    }
}
