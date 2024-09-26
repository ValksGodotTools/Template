namespace Template.Inventory;

public class InventorySlotContext(UICursorInventory uiCursorInventory, Inventory inv, ItemContainer itemContainer, int index)
{
    public CursorManager CursorManager { get; } = new(uiCursorInventory.Inventory, uiCursorInventory.CursorItemContainer);
    public InventoryManager InventoryManager { get; } = new(inv, index, itemContainer);
    public CursorInventory CursorInventory { get; } = uiCursorInventory.Inventory;
    public Inventory Inventory { get; } = inv;
    public ItemContainer ItemContainer { get; } = itemContainer;
    public int Index { get; } = index;
}
