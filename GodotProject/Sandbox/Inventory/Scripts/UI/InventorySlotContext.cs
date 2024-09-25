namespace Template.Inventory;

public class InventorySlotContext(Inventory inv, ItemContainer itemContainer, int index)
{
    public Inventory Inventory { get; } = inv;
    public ItemContainer ItemContainer { get; } = itemContainer;
    public int Index { get; } = index;
}
