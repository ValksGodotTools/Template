using Godot;

namespace Template.Inventory;

public partial class InventorySandbox : Node
{
	public override void _Ready()
	{
        Inventory inventory = new(10);
        inventory.AddItem(Items.Coin, 5);
        inventory.AddItem(Items.SnowyCoin, 3);
        inventory.SetItem(9, new Item(Items.Coin, 42));
        inventory.SwapItems(9, 0);
        inventory.RemoveItem(0);

        InventoryContainer inventoryContainer = new(inventory, this, 5);
	}
}
