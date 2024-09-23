using Godot;

namespace Template.Inventory;

public partial class InventorySandbox : Node
{
	public override void _Ready()
	{
        UIInventoryContainer inv = new(10, 5);

        for (int i = 0; i < 10; i++)
        {
            inv.SetItem(i, InventoryItems.YellowCoin());
        }

        AddChild(inv.Build());
	}
}
