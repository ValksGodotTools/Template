using Godot;

namespace Template.Inventory;

public class CursorManager(CursorItemContainer cursorItemContainer)
{
    public void SetItem(Item item, ItemContainer originItemContainer)
    {
        cursorItemContainer.SetPosition(originItemContainer.GlobalPosition);
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

    public void ClearItem(ItemContainer targetContainer)
    {
        if (targetContainer != null)
        {
            cursorItemContainer.StartLerpingTowardsTargetContainer(targetContainer);
        }
        else
        {
            cursorItemContainer.ClearItem();
        }
    }
}
