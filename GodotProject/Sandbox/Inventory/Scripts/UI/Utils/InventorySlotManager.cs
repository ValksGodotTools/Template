namespace Template.Inventory;

public class InventorySlotManager(InventorySlotContext context)
{
    public void GetItemAndFrame(out Item item, out int frame)
    {
        item = context.Inventory.GetItem(context.Index);
        frame = context.ItemContainer.GetCurrentSpriteFrame();
    }

    public void SetItemAndFrame(Item item, int frame)
    {
        context.Inventory.SetItem(context.Index, item);
        context.ItemContainer.SetCurrentSpriteFrame(frame);
    }
}
