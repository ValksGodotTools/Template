using Godot;
using System;

namespace Template.InventoryV2;

public partial class InventorySandbox : Node
{
    public override void _Ready()
    {
        Inventory inventory = new(size: 10);
        inventory.SetItem(0, Items.SnowyCoin, 3);
        inventory.SetItem(4, Items.Coin, 100);

        _ = new UIInventory(inventory, this);
    }
}
