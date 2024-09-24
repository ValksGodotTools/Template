using Godot;
using System.Collections.Generic;

namespace Template.InventoryV2;

public class UIInventory
{
    private List<ItemContainer> _itemContainers = [];

    public UIInventory(Inventory inventory, Node parent)
    {
        // Create the InventoryContainer
        InventoryContainer invContainer = InventoryContainer.Instantiate();

        // Add InventoryContainer to the tree as soon as possible so _Ready() functions in
        // children nodes will get called at the correct times
        parent.AddChild(invContainer);

        // Add the item containers
        AddItemContainers(invContainer, inventory);

        inventory.OnItemChanged += (index, item) =>
        {
            _itemContainers[index].SetItem(item);
        };
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
        _itemContainers.Add(itemContainer);

        Item item = inv.GetItem(index);

        if (item != null)
        {
            itemContainer.SetItem(item);
        }
    }
}
