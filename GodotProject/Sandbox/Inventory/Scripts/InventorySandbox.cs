using Godot;
using GodotUtils;

namespace Template.Inventory;

public partial class InventorySandbox : Node
{
    Inventory _inventory;

    public override void _Ready()
    {
        UICursorInventory cursorInventory = new(this);

        _inventory = new(size: 5);
        _inventory.AddItem(Items.SnowyCoin, 3);
        _inventory.AddItem(Items.Coin, 100);

        _ = new UIInventory(_inventory, cursorInventory, this);

        _inventory.AddItem(Items.SnowyCoin, 42);
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
