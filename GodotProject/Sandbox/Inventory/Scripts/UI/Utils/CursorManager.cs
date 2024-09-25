using Godot;

namespace Template.Inventory;

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
