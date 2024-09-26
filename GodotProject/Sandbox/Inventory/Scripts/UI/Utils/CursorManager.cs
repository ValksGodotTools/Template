using Godot;

namespace Template.Inventory;

public class CursorManager(CursorItemContainer cursorItemContainer)
{
    public Inventory Inventory { get => cursorItemContainer.Inventory; }

    public void SetItem(Item item, Vector2 position, int spriteFrame)
    {
        cursorItemContainer.SetPosition(position);
        cursorItemContainer.SetItemAndFrame(item, spriteFrame);
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

    public void GetItemAndFrame(out Item item, out int frame)
    {
        cursorItemContainer.GetItemAndFrame(out item, out frame);
    }

    public void ClearItem()
    {
        cursorItemContainer.ClearItem();
    }
}
