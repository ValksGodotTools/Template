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
                SwapItems(context);
            }
            else
            {
                PlaceItem(context);
            }
        }
        else
        {
            if (inv.HasItem(index))
            {
                PickupItem(context);
            }
        }
    }

    private void SwapItems(InventorySlotContext context)
    {
        // Get the cursor and inventory items
        context.CursorManager.GetItemAndFrame(out Item cursorItem, out int cursorItemFrame);
        context.InventoryManager.GetItemAndFrame(out Item invItem, out int invSpriteFrame);

        // Set the inv item with the cursor item
        context.InventoryManager.SetItemAndFrame(cursorItem, cursorItemFrame);

        // Set the cursor item with the inv item
        context.CursorManager.SetItem(invItem, context.ItemContainer.GlobalPosition, invSpriteFrame);
    }

    private void PlaceItem(InventorySlotContext context)
    {
        // Get the item and sprite frame before clearing the item from the cursor
        context.CursorManager.GetItemAndFrame(out Item cursorItem, out int cursorItemFrame);

        // Clear the item from the cursor
        context.CursorManager.ClearItem();

        // Set the inventory item
        context.InventoryManager.SetItemAndFrame(cursorItem, cursorItemFrame);
    }

    private void PickupItem(InventorySlotContext context)
    {
        // Get the item and sprite frame before clearing the item from the inventory
        context.InventoryManager.GetItemAndFrame(out Item item, out int frame);

        // Clear the item from the inventory
        context.Inventory.ClearItem(context.Index);

        // Set the cursor item
        context.CursorManager.SetItem(item, context.ItemContainer.GlobalPosition, frame);
    }
}
