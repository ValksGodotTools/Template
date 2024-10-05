using Godot;
using System.Collections.Generic;

namespace Template.Inventory;

public class InventoryVisualEffects(CanvasLayer _ui)
{
    private List<Node> _swapAnimContainers = [];

    public void AnimateDragPickup(CursorItemContainer cursorItemContainer, Inventory cursorInventory, Inventory inventory, ItemContainer[] itemContainers, int index)
    {
        AnimHelperItemContainer container = new AnimHelperItemContainer.Builder(AnimHelperItemContainer.Instantiate())
            .SetInitialPositionForControl(itemContainers[index].GlobalPosition)
            .SetTargetAsMouse()
            .SetStartingLerp(0.3f) // Need to make animation quick
            .SetItemAndFrame(inventory.GetItem(index), 0)
            .SetCount(0) // Too much information on screen gets chaotic
            .Build();

        _ui.AddChild(container);

        if (!cursorInventory.HasItem(0))
        {
            cursorItemContainer.HideSpriteAndCount();
        }

        container.OnReachedTarget += cursorItemContainer.ShowSpriteAndCount;
    }

    public void AnimateDragPlace(Vector2 mouseGlobalPosition, ItemContainer[] itemContainers, int index, Inventory cursorInventory)
    {
        // Place one of item from cursor to inventory slot
        AnimHelperItemContainer container = new AnimHelperItemContainer.Builder(AnimHelperItemContainer.Instantiate())
            .SetInitialPositionForNode2D(mouseGlobalPosition)
            .SetControlTarget(itemContainers[index].GlobalPosition)
            .SetItemAndFrame(cursorInventory.GetItem(0), 0)
            .SetCount(0) // Too much information on screen gets chaotic
            .Build();

        itemContainers[index].HideSpriteAndCount();

        container.OnReachedTarget += () =>
        {
            itemContainers[index].ShowSpriteAndCount();
        };

        _ui.AddChild(container);
    }

    public void AnimatePickup(ItemContainer[] itemContainers, int index, int itemFrame, Inventory inventory, CursorItemContainer cursorItemContainer)
    {
        AnimHelperItemContainer container = new AnimHelperItemContainer.Builder(AnimHelperItemContainer.Instantiate())
            .SetInitialPositionForControl(itemContainers[index].GlobalPosition)
            .SetTargetAsMouse()
            .SetItemAndFrame(inventory.GetItem(index), itemFrame)
            .Build();

        container.OnReachedTarget += cursorItemContainer.ShowSpriteAndCount;

        _ui.AddChild(container);
    }

    public void AnimatePlace(Vector2 globalMousePosition, ItemContainer[] itemContainers, int index, Inventory cursorInventory, int itemFrame)
    {
        AnimHelperItemContainer container = new AnimHelperItemContainer.Builder(AnimHelperItemContainer.Instantiate())
            .SetInitialPositionForNode2D(globalMousePosition)
            .SetControlTarget(itemContainers[index].GlobalPosition)
            .SetItemAndFrame(cursorInventory.GetItem(0), itemFrame)
            .Build();

        container.OnReachedTarget += () =>
        {
            itemContainers[index].ShowSpriteAndCount();
        };

        _ui.AddChild(container);
    }

    public void AnimateSwap(ItemContainer[] itemContainers, int index, Inventory inventory, int itemFrame, CursorItemContainer cursorItemContainer, Inventory cursorInventory, Vector2 globalMousePosition)
    {
        foreach (Node node in _swapAnimContainers)
        {
            if (GodotObject.IsInstanceValid(node))
            {
                node.QueueFree();
            }
        }

        _swapAnimContainers.Clear();

        AnimHelperItemContainer container = new AnimHelperItemContainer.Builder(AnimHelperItemContainer.Instantiate())
                .SetInitialPositionForControl(itemContainers[index].GlobalPosition)
                .SetTargetAsMouse()
                .SetItemAndFrame(inventory.GetItem(index), itemFrame)
                .Build();

        container.OnReachedTarget += cursorItemContainer.ShowSpriteAndCount;

        _ui.AddChild(container);

        AnimHelperItemContainer container2 = new AnimHelperItemContainer.Builder(AnimHelperItemContainer.Instantiate())
                .SetInitialPositionForNode2D(globalMousePosition)
                .SetControlTarget(itemContainers[index].GlobalPosition)
                .SetItemAndFrame(cursorInventory.GetItem(0), itemFrame)
                .Build();

        container2.OnReachedTarget += () =>
        {
            itemContainers[index].ShowSpriteAndCount();
        };

        _ui.AddChild(container2);

        _swapAnimContainers.Add(container);
        _swapAnimContainers.Add(container2);
    }
}
