namespace Template.Inventory;

public class InventorySlotContext(CursorItemContainer cursorItemContainer, Inventory inv, ItemContainer itemContainer, int index)
{
    public BaseInventory CursorInventory { get; } = cursorItemContainer.Inventory;
    public BaseInventory Inventory { get; } = inv;
    public ItemContainer ItemContainer { get; } = itemContainer;
    public int Index { get; } = index;
}
