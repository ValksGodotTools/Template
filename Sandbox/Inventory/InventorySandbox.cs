using Godot;
using GodotUtils;
using Layout = Godot.Control.LayoutPreset;

namespace Template.Inventory;

[SceneTree]
[Service]
public partial class InventorySandbox : Node
{
    private InventoryContainer _invContainerPlayer;
    private InventoryContainer _invContainerChest;

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
                _invContainerChest.Inventory.DebugPrint();
            }
            else if (key.IsJustPressed(Key.W))
            {
                _invContainerPlayer.Inventory.DebugPrint();
            }
        }
    }

    public InventoryContainer GetOtherInventory(InventoryContainer inventory)
    {
        return inventory == _invContainerPlayer ? _invContainerChest : _invContainerPlayer;
    }

    public InventoryContainer GetPlayerInventory()
    {
        return _invContainerPlayer;
    }

    private void AddPlayerInv()
    {
        Inventory inv = new(40);

        _invContainerPlayer = InventoryContainer.Instantiate(inv, columns: 10);

        InventoryParent.AddChild(_invContainerPlayer);
        InventoryParent.MoveChild(_invContainerPlayer, InventoryParent.GetChildCount() - 1); // appear bottom

        _invContainerPlayer.SetLayout(Layout.CenterBottom);
        _invContainerPlayer.Position -= new Vector2(0, 50);
    }

    private void AddChestInv()
    {
        Inventory inv = new(40);
        inv.AddItem(new ItemStack(Material.SnowyCoin, 3));
        inv.AddItem(new ItemStack(Material.Coin, 100));
        inv.SetItem(2, new ItemStack(Material.SnowyCoin, 42));
        inv.SetItem(3, new ItemStack(Material.Coin, 2));

        _invContainerChest = InventoryContainer.Instantiate(inv, columns: 10);

        InventoryParent.AddChild(_invContainerChest);
        InventoryParent.MoveChild(_invContainerChest, 0); // other inventory should always appear on top

        _invContainerChest.SetLayout(Layout.CenterTop);
        _invContainerChest.Position += new Vector2(0, 50);
    }
}
