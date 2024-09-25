using Godot;

namespace Template.InventoryV2;

public class CursorManager(CursorItemContainer cursorItemContainer)
{
    public void SetItem(Item item, Vector2 position)
    {
        cursorItemContainer.SetPosition(position);
        cursorItemContainer.SetItem(item);
    }

    public bool HasItem()
    {
        return cursorItemContainer.HasItem();
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
