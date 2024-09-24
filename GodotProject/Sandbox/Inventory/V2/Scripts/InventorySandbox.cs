using Godot;
using System;

namespace Template.InventoryV2;

public partial class InventorySandbox : Node
{
    public override void _Ready()
    {
        Inventory inventory = new(size: 10);
        inventory.SetItem(0, Items.SnowyCoin);
        inventory.SetItem(4, new Item(Items.Coin, 3));

        _ = new UIInventory(inventory, this);
    }
}

public class UIInventory
{
    public UIInventory(Inventory inventory, Node parent)
    {
        AddItemContainers(inventory, parent);
    }

    private void AddItemContainers(Inventory inventory, Node parent)
    {
        InventoryContainer inventoryContainer = InventoryContainer.Instantiate();

        parent.AddChild(inventoryContainer);

        for (int i = 0; i < inventory.GetInventorySize(); i++)
        {
            ItemContainer itemContainer = inventoryContainer.AddItemContainer();

            Item item = inventory.GetItem(i);

            if (item != null)
            {
                itemContainer.SetSpriteFrames(item.ResourcePath);
                itemContainer.SetCount(item.Count);
            }
        }
    }
}
