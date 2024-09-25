using Godot;
using System.Collections.Generic;

namespace Template.Inventory;

public class CursorItemContainerPool
{
    public List<CursorItemContainer> Pool { get; } = [];

    public CursorItemContainerPool(Node parent, int size)
    {
        InitializePool(parent, size);
    }

    private void InitializePool(Node parent, int size)
    {
        for (int i = 0; i < size; i++)
        {
            CursorItemContainer cursorItemContainer = CursorItemContainer.Instantiate();
            Pool.Add(cursorItemContainer);
            parent.AddChild(cursorItemContainer);
        }
    }

    public CursorItemContainer GetAvailableCursorItemContainer()
    {
        foreach (CursorItemContainer cursorItemContainer in Pool)
        {
            if (!cursorItemContainer.IsLerpingTowardsTargetContainer())
            {
                return cursorItemContainer;
            }
        }

        return null;
    }
}
