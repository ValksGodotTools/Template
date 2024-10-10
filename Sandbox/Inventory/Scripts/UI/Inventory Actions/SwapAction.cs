using Godot;

namespace Template.Inventory;

public class SwapAction : IInventoryAction
{
    public void Execute(InventoryContext context, MouseButton mouseBtn, int index)
    {
        if (mouseBtn == MouseButton.Left)
        {
            context.CursorInventory.MoveItemTo(context.Inventory, 0, index);
        }
        else if (mouseBtn == MouseButton.Right)
        {
            // Right click swap disabled because inteferes with right click drag
        }
    }
}
