using Godot;
using System;

namespace Template.Inventory;

public partial class InventorySandbox : Node
{
	public override void _Ready()
	{
        UIInventoryContainer inv = new(5);

        for (int i = 0; i < 10; i++)
        {
            inv.AddItem(InventoryItems.YellowCoin());
        }

        AddChild(inv.Build());
	}
}
