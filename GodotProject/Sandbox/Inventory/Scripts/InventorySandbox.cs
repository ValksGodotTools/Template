using Godot;

namespace Template.Inventory;

public partial class InventorySandbox : Node
{
	public override void _Ready()
	{
        Inventory inventroy = new();
        inventroy.AddItem(Items.Coin, 5);
        inventroy.AddItem(Items.SnowyCoin, 3);

        InventoryContainer inventoryContainer = new(inventroy, 5);
        AddChild(inventoryContainer.Build());
	}
}
