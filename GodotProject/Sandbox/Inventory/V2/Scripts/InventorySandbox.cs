using Godot;
using System;

namespace Template.InventoryV2;

public partial class InventorySandbox : Node
{
    public override void _Ready()
    {
        _ = new UIInventory(this);
    }
}

public class UIInventory
{
    public UIInventory(Node parent)
    {
        AddItemContainers(parent);
    }

    private void AddItemContainers(Node parent)
    {
        InventoryContainer inventoryContainer = InventoryContainer.Instantiate();

        for (int i = 0; i < 10; i++)
        {
            inventoryContainer.AddItemContainer();
        }

        parent.AddChild(inventoryContainer);
    }
}
