namespace Template.Inventory;

public class InventorySlotContext(CursorManager cursorManager, Inventory inv, ItemContainer itemContainer, int index)
{
    public CursorManager CursorManager { get; } = cursorManager;
    public Inventory Inventory { get; } = inv;
    public ItemContainer ItemContainer { get; } = itemContainer;
    public int Index { get; } = index;
    public InventoryManager InventoryManager { get; } = new(inv, index, itemContainer);
}
