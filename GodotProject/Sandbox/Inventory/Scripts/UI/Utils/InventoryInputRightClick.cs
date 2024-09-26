using Godot;
using GodotUtils;

namespace Template.Inventory;

public class InventoryInputRightClick : InventoryInputHandler
{
    public override bool HasInput(InputEventMouseButton mouseBtn)
    {
        return mouseBtn.IsRightClickPressed();
    }

    // Place one item from the cursor onto the inventory item
    public override void HandleSameType(InventorySlotContext context)
    {
        TakeAmountFromCursorAndPutOnInvItem(context, 1);
    }

    // Swap the cursor item with the inventory item
    public override void HandleDiffType(InventorySlotContext context)
    {
        base.HandleDiffType(context);
    }

    // Place one item from the cursor to the inventory
    public override void HandlePlace(InventorySlotContext context)
    {
        // Get the item and sprite frame before clearing the item from the cursor
        context.CursorManager.GetItemAndFrame(out Item cursorItem, out int cursorItemFrame);

        // Create a new item with a count of 1
        Item newItem = new(cursorItem);
        newItem.SetCount(1);

        // Reduce the cursor item count by 1
        cursorItem.RemoveCount(1);

        // Set the inventory item
        context.InventoryManager.SetItemAndFrame(newItem, cursorItemFrame);
    }

    // Pickup half of the item from the inventory and put it on the cursor, or a single item if only one exists
    public override void HandlePickup(InventorySlotContext context)
    {
        // Get the item and sprite frame before clearing the item from the inventory
        context.InventoryManager.GetItemAndFrame(out Item invItem, out int invSpriteFrame);

        // Calculate the half count to split
        int halfCount = invItem.Count / 2;

        // If the inventory item count is 1, pick up a single item
        if (invItem.Count == 1)
        {
            halfCount = 1;
        }

        // Create a new item with the half count
        Item newItem = new(invItem);
        newItem.SetCount(halfCount);

        // Reduce the inventory item count by the half count
        invItem.RemoveCount(halfCount);

        // Set the cursor item
        context.CursorManager.SetItem(newItem, context.ItemContainer.GlobalPosition, invSpriteFrame);
    }
}
