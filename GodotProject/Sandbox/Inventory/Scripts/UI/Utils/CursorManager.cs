using Godot;

namespace Template.Inventory;

public class CursorManager(Inventory inv, CursorItemContainer cursorItemContainer) : InventoryManager(inv, 0, cursorItemContainer)
{
    public Inventory Inventory { get => cursorItemContainer.Inventory; }

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

    public bool HasItem()
    {
        return cursorItemContainer.HasItem();
    }

    public bool ItemsAreOfSameType(Item itemToCompare)
    {
        return cursorItemContainer.GetItem().Equals(itemToCompare);
    }

    public Item GetItem()
    {
        return cursorItemContainer.GetItem();
    }

    public void ClearItem()
    {
        cursorItemContainer.ClearItem();
    }
}
