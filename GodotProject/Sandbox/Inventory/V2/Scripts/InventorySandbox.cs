using Godot;

namespace Template.InventoryV2;

public partial class InventorySandbox : Node
{
    public override void _Ready()
    {
        Inventory inventory = new(size: 10);
        inventory.AddItem(Items.SnowyCoin, 3);
        inventory.AddItem(Items.Coin, 100);

        _ = new UIInventory(inventory, this);

        inventory.AddItem(Items.SnowyCoin, 42);
    }
}
