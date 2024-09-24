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
