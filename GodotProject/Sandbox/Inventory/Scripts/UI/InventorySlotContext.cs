namespace Template.Inventory;

public class InventorySlotContext
{
    public CursorManager CursorManager { get; }
    public Inventory Inventory { get; }
    public ItemContainer ItemContainer { get; }
    public int Index { get; }
    public InventorySlotManager InventoryManager { get; }

    public InventorySlotContext(CursorManager cursorManager, Inventory inv, ItemContainer itemContainer, int index)
    {
        CursorManager = cursorManager;
        Inventory = inv;
        ItemContainer = itemContainer;
        Index = index;
        InventoryManager = new InventorySlotManager(this);
    }
}
