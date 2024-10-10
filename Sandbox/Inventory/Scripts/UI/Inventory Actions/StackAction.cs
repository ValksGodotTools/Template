using Godot;

namespace Template.Inventory;

public class StackAction : IInventoryAction
{
    public void Execute(InventoryContext context, MouseButton mouseBtn, int index)
    {
        if (mouseBtn == MouseButton.Left)
        {
            // Stack the entire cursor item stack
            context.CursorInventory.MovePartOfItemTo(context.Inventory, 0, index, context.CursorInventory.GetItem(0).Count);
        }
        else if (mouseBtn == MouseButton.Right)
        {
            // Stack a single item
            context.CursorInventory.MovePartOfItemTo(context.Inventory, 0, index, 1);
        }
    }
}
