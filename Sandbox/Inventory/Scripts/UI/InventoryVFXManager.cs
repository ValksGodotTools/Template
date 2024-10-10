using Godot;

namespace Template.Inventory;

public class InventoryVFXManager
{
    public void RegisterEvents(InventoryInputHandler input, InventoryContext context, InventoryContainer container)
    {
        AnimHelperItemContainer vfxContainer = null;

        int itemFrame = 0;
        int cursorFrame = 0;

        input.OnPreInventoryAction += args =>
        {
            if (args.Action == InventoryAction.Pickup)
            {
                itemFrame = args.TargetInventoryContainer.ItemContainers[args.FromIndex].GetCurrentSpriteFrame();

                vfxContainer = context.VFX.AnimatePickup(args.TargetInventoryContainer, context, args.FromIndex, itemFrame);
            }
            else if (args.Action == InventoryAction.Place)
            {
                itemFrame = context.CursorItemContainer.GetCurrentSpriteFrame();

                vfxContainer = context.VFX.AnimatePlace(context, args.FromIndex, itemFrame, container.GetGlobalMousePosition());
            }
            else if (args.Action == InventoryAction.Stack)
            {
                itemFrame = context.ItemContainers[args.FromIndex].GetCurrentSpriteFrame();
            }
            else if (args.Action == InventoryAction.Swap)
            {
                itemFrame = context.ItemContainers[args.FromIndex].GetCurrentSpriteFrame();
                cursorFrame = context.CursorItemContainer.GetCurrentSpriteFrame();

                context.VFX.AnimateSwap(context, args.FromIndex, itemFrame, container.GetGlobalMousePosition());
            }
            else if (args.Action == InventoryAction.Transfer)
            {
                context.VFX.AnimateTransfer(context, args.TargetInventoryContainer.ItemContainers[args.ToIndex], args.FromIndex);
            }
        };

        input.OnPostInventoryAction += args =>
        {
            if (args.Action == InventoryAction.Pickup)
            {
                // Ensure the count is correctly displayed
                vfxContainer.SetCount(context.CursorInventory.GetItem(0).Count);

                CursorItemContainer cursorItemContainer = context.CursorItemContainer;

                cursorItemContainer.HideSpriteAndCount(); // Needed for visual effects to work
                cursorItemContainer.SetCurrentSpriteFrame(itemFrame);

                // Ensure cursorItemContainer's position is in the correct position
                cursorItemContainer.Position = args.TargetInventoryContainer.ItemContainers[args.FromIndex].GlobalPosition;
            }
            else if (args.Action == InventoryAction.Place)
            {
                // Ensure the count is correctly displayed
                vfxContainer.SetCount(context.Inventory.GetItem(args.FromIndex).Count);

                context.ItemContainers[args.FromIndex].HideSpriteAndCount(); // Needed for visual effects to work
                context.ItemContainers[args.FromIndex].SetCurrentSpriteFrame(itemFrame);
            }
            else if (args.Action == InventoryAction.Stack)
            {
                context.ItemContainers[args.FromIndex].SetCurrentSpriteFrame(itemFrame);
            }
            else if (args.Action == InventoryAction.Swap)
            {
                context.ItemContainers[args.FromIndex].HideSpriteAndCount(); // Needed for visual effects to work
                context.CursorItemContainer.HideSpriteAndCount(); // Needed for visual effects to work

                context.ItemContainers[args.FromIndex].SetCurrentSpriteFrame(cursorFrame);
                context.CursorItemContainer.SetCurrentSpriteFrame(itemFrame);

                // Ensure cursorItemContainer's position is in the correct position
                context.CursorItemContainer.Position = context.ItemContainers[args.FromIndex].GlobalPosition;
            }
            else if (args.Action == InventoryAction.Transfer)
            {
                if (!args.AreSameType)
                {
                    args.TargetInventoryContainer.ItemContainers[args.ToIndex].HideSpriteAndCount();
                }
            }
        };
    }

    public void AnimateDragPickup(InventoryContext context, int index)
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

    public void AnimateDragPlace(InventoryContext context, int index, Vector2 mousePos)
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
