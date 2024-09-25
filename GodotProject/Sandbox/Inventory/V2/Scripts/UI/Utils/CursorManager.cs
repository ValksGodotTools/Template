using Godot;

namespace Template.InventoryV2;

public class CursorManager(CursorItemContainer cursorItemContainer)
{
    public void SetCursorItem(Item item, Vector2 position)
    {
        cursorItemContainer.SetPosition(position);
        cursorItemContainer.SetItem(item);
    }
}
