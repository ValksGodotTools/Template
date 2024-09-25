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

        // Get the item from the cursor
        cursorManager.GetItemAndFrame(out Item cursorItem, out int cursorItemFrame);

        // Get the item from the inventory
        Inventory inv = context.Inventory;
        Item invItem = inv.GetItem(context.Index);
        int invSpriteFrame = context.ItemContainer.GetCurrentSpriteFrame();

        // Set the inv item with the cursor item
        inv.SetItem(context.Index, cursorItem);
        context.ItemContainer.SetCurrentSpriteFrame(cursorItemFrame);

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
        context.Inventory.SetItem(context.Index, cursorItem);
        context.ItemContainer.SetCurrentSpriteFrame(cursorItemFrame);
    }

    private void HandlePickupItem(InventorySlotContext context)
    {
        Inventory inv = context.Inventory;

        // Get the item and sprite frame before clearing the item from the inventory
        Item item = inv.GetItem(context.Index);
        int spriteFrame = context.ItemContainer.GetCurrentSpriteFrame();

        // Clear the item from the inventory
        inv.ClearItem(context.Index);

        // Set the cursor item
        context.CursorManager.SetItem(item, context.ItemContainer.GlobalPosition, spriteFrame);
    }
}
