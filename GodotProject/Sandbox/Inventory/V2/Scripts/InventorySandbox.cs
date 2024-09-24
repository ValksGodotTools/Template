using Godot;
using System;

namespace Template.InventoryV2;

public partial class InventorySandbox : Node
{
    public override void _Ready()
    {
        Inventory inventory = new(size: 10);

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

        for (int i = 0; i < inventory.GetInventorySize(); i++)
        {
            inventoryContainer.AddItemContainer();
        }

        parent.AddChild(inventoryContainer);
    }
}
