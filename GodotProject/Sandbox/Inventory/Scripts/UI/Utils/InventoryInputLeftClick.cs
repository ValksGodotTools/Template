using Godot;
using GodotUtils;

namespace Template.Inventory;

public class InventoryInputLeftClick : InventoryInputHandler
{
    public override bool HasInput(InputEventMouseButton mouseBtn)
    {
        return mouseBtn.IsLeftClickPressed();
    }

    // Stack the cursor item onto the inventory item
    public override void HandleSameType(InventorySlotContext context)
    {
        TakeAmountFromCursorAndPutOnInvItem(context, context.CursorManager.GetItem().Count);
    }

    public override void HandleDiffType(InventorySlotContext context)
    {
        base.HandleDiffType(context);
    }

    // Place the item from the cursor to the inventory
    public override void HandlePlace(InventorySlotContext context)
    {
        // Get the item and sprite frame before clearing the item from the cursor
        context.CursorManager.GetItemAndFrame(out Item cursorItem, out int cursorItemFrame);

        // Clear the item from the cursor
        context.CursorManager.ClearItem();

        // Set the inventory item
        context.InventoryManager.SetItemAndFrame(cursorItem, cursorItemFrame);
    }

    // Pickup the item from the inventory and put it on the cursor
    public override void HandlePickup(InventorySlotContext context)
    {
        // Get the item and sprite frame before clearing the item from the inventory
        context.InventoryManager.GetItemAndFrame(out Item item, out int frame);

        // Clear the item from the inventory
        context.Inventory.ClearItem(context.Index);

        // Set the cursor item
        context.CursorManager.SetItem(item, context.ItemContainer.GlobalPosition, frame);
    }
}
