namespace Template.Inventory;

public class InventoryInputHandler()
{
    public void HandleLeftClick(InventorySlotContext context)
    {
        Inventory inv = context.Inventory;
        int index = context.Index;

        if (context.CursorManager.HasItem())
        {
            if (inv.HasItem(index))
            {
                HandleSwapItems(context);
            }
            else
            {
                HandlePlaceItem(context);
            }
        }
        else
        {
            if (inv.HasItem(index))
            {
                HandlePickupItem(context);
            }
        }
    }

    private void HandleSwapItems(InventorySlotContext context)
    {
        CursorManager cursorManager = context.CursorManager;
        InventorySlotManager inventoryManager = context.InventoryManager;

        // Get the cursor and inventory items
        cursorManager.GetItemAndFrame(out Item cursorItem, out int cursorItemFrame);
        inventoryManager.GetItemAndFrame(out Item invItem, out int invSpriteFrame);

        // Set the inv item with the cursor item
        inventoryManager.SetItemAndFrame(cursorItem, cursorItemFrame);

        // Set the cursor item with the inv item
        cursorManager.SetItem(invItem, context.ItemContainer.GlobalPosition, invSpriteFrame);
    }

    private void HandlePlaceItem(InventorySlotContext context)
    {
        CursorManager cursorManager = context.CursorManager;

        // Get the item and sprite frame before clearing the item from the cursor
        cursorManager.GetItemAndFrame(out Item cursorItem, out int cursorItemFrame);

        // Clear the item from the cursor
        cursorManager.ClearItem();

        // Set the inventory item
        context.InventoryManager.SetItemAndFrame(cursorItem, cursorItemFrame);
    }

    private void HandlePickupItem(InventorySlotContext context)
    {
        // Get the item and sprite frame before clearing the item from the inventory
        context.InventoryManager.GetItemAndFrame(out Item item, out int frame);

        // Clear the item from the inventory
        context.Inventory.ClearItem(context.Index);

        // Set the cursor item
        context.CursorManager.SetItem(item, context.ItemContainer.GlobalPosition, frame);
    }
}
