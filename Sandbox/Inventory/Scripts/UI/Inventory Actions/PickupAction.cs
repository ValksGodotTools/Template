using Godot;

namespace Template.Inventory;

public class PickupAction : IInventoryAction
{
    public void Execute(InventoryContext context, MouseButton mouseBtn, int index)
    {
        if (mouseBtn == MouseButton.Left)
        {
            // Left click pickup logic
            context.CursorInventory.TakeItemFrom(context.Inventory, index, 0);
        }
        else if (mouseBtn == MouseButton.Right)
        {
            // Right click pickup (half stack or one item)
            Inventory cursorInventory = context.CursorInventory;
            Inventory inventory = context.Inventory;

            int halfItemCount = inventory.GetItem(index).Count / 2;

            if (context.InputDetector.HoldingShift && halfItemCount != 0)
            {
                cursorInventory.TakePartOfItemFrom(inventory, index, 0, halfItemCount);
            }
            else
            {
                cursorInventory.TakePartOfItemFrom(inventory, index, 0, 1);
            }
        }
    }
}
