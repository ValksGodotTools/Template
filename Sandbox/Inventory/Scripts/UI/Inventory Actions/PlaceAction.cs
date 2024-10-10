using Godot;

namespace Template.Inventory;

public class PlaceAction : IInventoryAction
{
    public void Execute(InventoryContext context, MouseButton mouseBtn, int index)
    {
        if (mouseBtn == MouseButton.Left)
        {
            // Place the whole stack
            context.CursorInventory.MoveItemTo(context.Inventory, 0, index);
        }
        else if (mouseBtn == MouseButton.Right)
        {
            // Place one item
            context.CursorInventory.MovePartOfItemTo(context.Inventory, 0, index, 1);
        }
    }
}
