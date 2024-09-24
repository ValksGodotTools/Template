using Godot;

namespace Template.Inventory;

public partial class InventorySandbox : Node
{
    private Inventory _inventory;

    public override void _Ready()
	{
        _inventory = new(10);
        _inventory.AddItem(Items.Coin, 5);
        _inventory.AddItem(Items.Coin, 3);
        _inventory.AddItem(Items.SnowyCoin, 3);
        _inventory.AddItem(Items.SnowyCoin, 2);

        InventoryContainer inventoryContainer = new(_inventory, this, 5);
	}

    public override void _Input(InputEvent @event)
    {
        if (@event is InputEventKey key)
        {
            if (key.Keycode == Key.Q)
            {
                _inventory.PrintInventory();
            }
        }
    }
}
