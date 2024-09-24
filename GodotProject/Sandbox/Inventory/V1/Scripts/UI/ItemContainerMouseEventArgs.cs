namespace Template.InventoryV1;

public class ItemContainerMouseEventArgs(int index, InventoryItemContainer inventoryItemContainer)
{
    public int Index { get; } = index;
    public InventoryItemContainer InventoryItemContainer { get; } = inventoryItemContainer;
}
