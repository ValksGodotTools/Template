using Godot;

namespace Template.InventoryV2;

public class UIInventory
{
    public UIInventory(Inventory inventory, Node parent)
    {
        // Create the InventoryContainer
        InventoryContainer invContainer = InventoryContainer.Instantiate();

        // Add InventoryContainer to the tree as soon as possible so _Ready() functions in
        // children nodes will get called at the correct times
        parent.AddChild(invContainer);

        // Add the item containers
        AddItemContainers(invContainer, inventory);
    }

    private void AddItemContainers(InventoryContainer invContainer, Inventory inv)
    {
        for (int i = 0; i < inv.GetInventorySize(); i++)
        {
            AddItemContainer(invContainer, inv, i);
        }
    }

    private void AddItemContainer(InventoryContainer invContainer, Inventory inv, int index)
    {
        ItemContainer itemContainer = invContainer.AddItemContainer();

        Item item = inv.GetItem(index);

        if (item != null)
        {
            itemContainer.SetItem(item);
        }
    }
}
