using Godot;
using GodotUtils;
using Layout = Godot.Control.LayoutPreset;

namespace Template.Inventory;

[SceneTree]
[Service]
public partial class InventorySandbox : Node
{
    private Inventory _invPlayer;
    private Inventory _invChest;

    public override void _Ready()
    {
        AddPlayerInv();
        AddChestInv();
    }

    public override void _Input(InputEvent @event)
    {
        if (@event is InputEventKey key)
        {
            if (key.IsJustPressed(Key.Q))
            {
                _invChest.DebugPrintInventory();
            }
            else if (key.IsJustPressed(Key.W))
            {
                _invPlayer.DebugPrintInventory();
            }
        }
    }

    public Inventory GetOtherInventory(Inventory inventory)
    {
        return inventory == _invPlayer ? _invChest : _invPlayer;
    }

    private void AddPlayerInv()
    {
        _invPlayer = new(40);

        InventoryContainer container = InventoryContainer.Instantiate(_invPlayer, columns: 10);

        InventoryParent.AddChild(container);
        InventoryParent.MoveChild(container, InventoryParent.GetChildCount() - 1); // appear bottom

        container.SetLayout(Layout.CenterBottom);
        container.Position -= new Vector2(0, 50);
    }

    private void AddChestInv()
    {
        _invChest = new(40);
        _invChest.AddItem(new ItemStack(Material.SnowyCoin, 3));
        _invChest.AddItem(new ItemStack(Material.Coin, 100));
        _invChest.SetItem(2, new ItemStack(Material.SnowyCoin, 42));
        _invChest.SetItem(3, new ItemStack(Material.Coin, 2));

        InventoryContainer container = InventoryContainer.Instantiate(_invChest, columns: 10);

        InventoryParent.AddChild(container);
        InventoryParent.MoveChild(container, 0); // other inventory should always appear on top

        container.SetLayout(Layout.CenterTop);
        container.Position += new Vector2(0, 50);
    }
}
