using Godot;
using GodotUtils;

namespace Template.Inventory;

public partial class InventorySandbox : Node
{
    Inventory _inventory;

    public override void _Ready()
    {
        _inventory = new(4);
        _inventory.AddItem(Items.SnowyCoin, 3);
        _inventory.AddItem(Items.Coin, 100);
        _inventory.AddItem(Items.SnowyCoin, 42);

        InventoryContainer invContainer = InventoryContainer.Instantiate(_inventory);
        AddChild(invContainer);
    }

    public override void _Input(InputEvent @event)
    {
        if (@event is InputEventKey key)
        {
            if (key.IsJustPressed(Key.Q))
            {
                _inventory.DebugPrintInventory();
            }
        }
    }
}
