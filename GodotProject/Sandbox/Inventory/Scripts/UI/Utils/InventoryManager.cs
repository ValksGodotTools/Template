namespace Template.Inventory;

public class InventoryManager(Inventory inv, int index, ItemContainer itemContainer)
{
    public virtual void GetItemAndFrame(out Item item, out int frame)
    {
        item = inv.GetItem(index);
        frame = itemContainer.GetCurrentSpriteFrame();
    }

    public virtual void SetItemAndFrame(Item item, int frame)
    {
        inv.SetItem(index, item);
        itemContainer.SetCurrentSpriteFrame(frame);
    }
}
