using Godot;
using GodotUtils;

namespace Template.Inventory;

public partial class InventorySandbox : Node
{
    Inventory _inventory;

    public override void _Ready()
    {
        _inventory = new(10);
        _inventory.AddItem(new ItemStack(Material.SnowyCoin, 3));
        _inventory.AddItem(new ItemStack(Material.Coin, 100));
        _inventory.SetItem(2, new ItemStack(Material.SnowyCoin, 42));
        _inventory.SetItem(3, new ItemStack(Material.Coin, 2));
        _inventory.SetItem(4, new ItemStack(Material.Coin, 2));
        _inventory.SetItem(5, new ItemStack(Material.Coin, 2));
        _inventory.SetItem(6, new ItemStack(Material.Coin, 2));

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
