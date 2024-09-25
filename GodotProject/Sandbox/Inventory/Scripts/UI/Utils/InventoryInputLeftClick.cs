using Godot;
using GodotUtils;

namespace Template.Inventory;

public class InventoryInputLeftClick : InventoryInputHandler
{
    public override bool CheckInput(InputEventMouseButton mouseBtn)
    {
        return mouseBtn.IsLeftClickPressed();
    }

    // Stack the cursor item onto the inventory item
    public override void HandleSameType(InventorySlotContext context)
    {
        // Get the cursor and inventory items
        context.InventoryManager.GetItemAndFrame(out Item invItem, out int invSpriteFrame);

        // Add the count from cursor item to inventory item
        invItem.Count += context.CursorManager.GetItem().Count;
        context.CursorManager.ClearItem();

        // Set the item with the new count
        context.InventoryManager.SetItemAndFrame(invItem, invSpriteFrame);
    }

    // Swap the cursor item with the inventory item
    public override void HandleDiffType(InventorySlotContext context)
    {
        // Get the cursor and inventory items
        context.CursorManager.GetItemAndFrame(out Item cursorItem, out int cursorItemFrame);
        context.InventoryManager.GetItemAndFrame(out Item invItem, out int invSpriteFrame);

        // Set the inv item with the cursor item
        context.InventoryManager.SetItemAndFrame(cursorItem, cursorItemFrame);

        // Set the cursor item with the inv item
        context.CursorManager.SetItem(invItem, context.ItemContainer.GlobalPosition, invSpriteFrame);
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
