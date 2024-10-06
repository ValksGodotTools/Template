using Godot;

namespace Template.Inventory;

public class InventoryVFXManager
{
    public void RegisterEvents(InventoryContainer container, InventoryVFXContext context)
    {
        AnimHelperItemContainer vfxContainer = null;

        int itemFrame = 0;
        int cursorFrame = 0;

        container.OnPrePickup += (clickType, index) =>
        {
            itemFrame = context.ItemContainers[index].GetCurrentSpriteFrame();

            vfxContainer = context.VFX.AnimatePickup(context, index, itemFrame);
        };

        container.OnPostPickup += (clickType, index) =>
        {
            // Ensure the count is correctly displayed
            vfxContainer.SetCount(context.CursorInventory.GetItem(0).Count);

            CursorItemContainer cursorItemContainer = context.CursorItemContainer;

            cursorItemContainer.HideSpriteAndCount(); // Needed for visual effects to work
            cursorItemContainer.SetCurrentSpriteFrame(itemFrame);

            // Ensure cursorItemContainer's position is in the correct position
            cursorItemContainer.Position = context.ItemContainers[index].GlobalPosition;
        };

        container.OnPrePlace += (clickType, index) =>
        {
            itemFrame = context.CursorItemContainer.GetCurrentSpriteFrame();

            vfxContainer = context.VFX.AnimatePlace(context, index, itemFrame, container.GetGlobalMousePosition());
        };

        container.OnPostPlace += (clickType, index) =>
        {
            // Ensure the count is correctly displayed
            vfxContainer.SetCount(context.Inventory.GetItem(index).Count);

            context.ItemContainers[index].HideSpriteAndCount(); // Needed for visual effects to work
            context.ItemContainers[index].SetCurrentSpriteFrame(itemFrame);
        };

        container.OnPreSwap += (clickType, index) =>
        {
            itemFrame = context.ItemContainers[index].GetCurrentSpriteFrame();
            cursorFrame = context.CursorItemContainer.GetCurrentSpriteFrame();

            context.VFX.AnimateSwap(context, index, itemFrame, container.GetGlobalMousePosition());
        };

        container.OnPostSwap += (clickType, index) =>
        {
            context.ItemContainers[index].HideSpriteAndCount(); // Needed for visual effects to work
            context.CursorItemContainer.HideSpriteAndCount(); // Needed for visual effects to work

            context.ItemContainers[index].SetCurrentSpriteFrame(cursorFrame);
            context.CursorItemContainer.SetCurrentSpriteFrame(itemFrame);

            // Ensure cursorItemContainer's position is in the correct position
            context.CursorItemContainer.Position = context.ItemContainers[index].GlobalPosition;
        };

        container.OnPreStack += (clickType, index) =>
        {
            itemFrame = context.ItemContainers[index].GetCurrentSpriteFrame();
        };

        container.OnPostStack += (clickType, index) =>
        {
            context.ItemContainers[index].SetCurrentSpriteFrame(itemFrame);
        };
    }

    public void AnimateDragPickup(InventoryVFXContext context, int index)
    {
        Inventory cursorInventory = context.CursorInventory;
        Inventory inventory = context.Inventory;

        if (cursorInventory.HasItem(0) && !cursorInventory.GetItem(0).Material.Equals(inventory.GetItem(index).Material))
        {
            // Do nothing
        }
        else
        {
            context.VFX.AnimateDragPickup(context, index);
        }
    }

    public void AnimateDragPlace(InventoryVFXContext context, int index, Vector2 mousePos)
    {
        Inventory cursorInventory = context.CursorInventory;
        Inventory inventory = context.Inventory;

        // Only do animations when the cursor has a item and the inventory does
        // not have an item. Otherwise too many animations gets too visually
        // chaotic.
        if (cursorInventory.HasItem(0) && !inventory.HasItem(index))
        {
            context.VFX.AnimateDragPlace(context, index, mousePos);
        }
    }
}
