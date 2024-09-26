namespace Template.Inventory;

public class InventorySlotContext(CursorItemContainer cursorItemContainer, Inventory inv, ItemContainer itemContainer, int index)
{
    public CursorManager CursorManager { get; } = new(cursorItemContainer.Inventory, cursorItemContainer);
    public InventoryManager InventoryManager { get; } = new(inv, index, itemContainer);
    public CursorInventory CursorInventory { get; } = cursorItemContainer.Inventory;
    public Inventory Inventory { get; } = inv;
    public ItemContainer ItemContainer { get; } = itemContainer;
    public int Index { get; } = index;
}
