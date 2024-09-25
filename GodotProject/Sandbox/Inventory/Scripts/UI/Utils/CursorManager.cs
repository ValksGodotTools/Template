using Godot;

namespace Template.Inventory;

public class CursorManager(CursorItemContainer cursorItemContainer)
{
    public void SetItem(Item item, Vector2 position, int spriteFrame)
    {
        cursorItemContainer.SetPosition(position);
        cursorItemContainer.SetItem(item);
        cursorItemContainer.SetCurrentSpriteFrame(spriteFrame);
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

    public int GetCurrentSpriteFrame()
    {
        return cursorItemContainer.GetCurrentSpriteFrame();
    }
}
