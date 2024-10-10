using Godot;

namespace Template.Inventory;

public class TransferAction : IInventoryAction
{
    public void Execute(InventoryContext context, MouseButton mouseBtn, int index)
    {
        if (mouseBtn == MouseButton.Left)
        {
            Transfer(context, index);
        }
    }

    public static void Transfer(InventoryContext context, int index)
    {
        InventoryContainer otherInventoryContainer = Services.Get<InventorySandbox>().GetOtherInventory(context.InventoryContainer);
        Inventory otherInventory = otherInventoryContainer.Inventory;

        if (otherInventory.TryFindFirstSameType(context.Inventory.GetItem(index).Material, out int stackIndex))
        {
            Transfer(context, true, otherInventoryContainer, otherInventory, index, stackIndex);
        }
        else if (otherInventory.TryFindFirstEmptySlot(out int otherIndex))
        {
            Transfer(context, false, otherInventoryContainer, otherInventory, index, otherIndex);
        }
    }

    private static void Transfer(InventoryContext context, bool areSameType, InventoryContainer otherInventoryContainer, Inventory otherInventory, int index, int otherIndex)
    {
        ItemContainer targetItemContainer = otherInventoryContainer.ItemContainers[otherIndex];

        TransferEventArgs args = new(areSameType, index, targetItemContainer);

        //OnPreTransfer?.Invoke(args);

        context.Inventory.MoveItemTo(otherInventory, index, otherIndex);

        //OnPostTransfer?.Invoke(args);
    }
}
