namespace Template.InventoryV2;

public class InventorySlotContext(Inventory inv, ItemContainer itemContainer, int index)
{
    public Inventory Inventory { get; } = inv;
    public ItemContainer ItemContainer { get; } = itemContainer;
    public int Index { get; } = index;
}
