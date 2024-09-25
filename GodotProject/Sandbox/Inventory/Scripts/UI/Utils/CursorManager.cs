using Godot;

namespace Template.Inventory;

public class CursorManager(Node parent, int poolSize)
{
    private CursorItemContainerPool _pool = new(parent, poolSize);

    public void SetItem(Item item, ItemContainer originItemContainer)
    {
        CursorItemContainer cursorItemContainer = _pool.GetAvailableCursorItemContainer();

        cursorItemContainer.SetPosition(originItemContainer.GlobalPosition);
        cursorItemContainer.SetItem(item);
    }

    public bool HasActiveItem()
    {
        foreach (CursorItemContainer cursorItemContainer in _pool.Pool)
        {
            if (cursorItemContainer.HasItem() && !cursorItemContainer.IsLerpingTowardsTargetContainer())
            {
                return true;
            }
        }

        return false;
    }

    public Item GetItem()
    {
        foreach (CursorItemContainer cursorItemContainer in _pool.Pool)
        {
            if (cursorItemContainer.HasItem())
            {
                return cursorItemContainer.GetItem();
            }
        }

        return null;
    }

    public void ClearItem(ItemContainer targetContainer)
    {
        foreach (CursorItemContainer cursorItemContainer in _pool.Pool)
        {
            if (cursorItemContainer.HasItem())
            {
                if (targetContainer != null)
                {
                    cursorItemContainer.StartLerpingTowardsTargetContainer(targetContainer);
                }
                else
                {
                    cursorItemContainer.ClearItem();
                }

                break;
            }
        }
    }
}
