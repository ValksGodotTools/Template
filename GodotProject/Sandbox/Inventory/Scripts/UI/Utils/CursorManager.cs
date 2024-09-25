using Godot;

namespace Template.Inventory;

public class CursorManager(CursorItemContainer cursorItemContainer)
{
    private ItemContainer _originItemContainer;
    private ItemContainer _targetItemContainer;

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
        if (_targetItemContainer != null)
        {
            cursorItemContainer.StartLerpingTowardsTargetContainer(_targetItemContainer);
        }
        else
        {
            cursorItemContainer.ClearItem();
        }
    }

    public void SetTargetContainer(ItemContainer targetContainer)
    {
        _targetItemContainer = targetContainer;
    }
}
