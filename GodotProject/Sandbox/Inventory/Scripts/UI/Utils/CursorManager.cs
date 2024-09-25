using Godot;

namespace Template.Inventory;

public class CursorManager(CursorItemContainer cursorItemContainer)
{
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

    public void GetItemAndFrame(out Item item, out int frame)
    {
        cursorItemContainer.GetItemAndFrame(out item, out frame);
    }

    public void ClearItem()
    {
        cursorItemContainer.ClearItem();
    }
}
