using Godot;
using GodotUtils;

namespace Template.Inventory;

public class InventoryInputRightClick : InventoryInputHandler
{
    public override bool HasInput(InputEventMouseButton mouseBtn)
    {
        return mouseBtn.IsRightClickPressed();
    }

    // Place one item from the cursor onto the inventory item
    public override void HandleSameType(InventorySlotContext context)
    {
        InputCommon.HandleSameType(context, 1);
    }

    // Swap the cursor item with the inventory item
    public override void HandleDiffType(InventorySlotContext context)
    {
        base.HandleDiffType(context);
    }

    // Place one item from the cursor to the inventory
    public override void HandlePlace(InventorySlotContext context)
    {
        InputCommon.HandlePlace(context, 1);
    }

    // Pickup half of the item from the inventory and put it on the cursor, or a single item if only one exists
    public override void HandlePickup(InventorySlotContext context)
    {
        Item item = context.Inventory.GetItem(context.Index);

        int halfCount = item.Count / 2;

        if (item.Count == 1)
        {
            halfCount = 1;
        }

        InputCommon.HandlePickup(context, halfCount);
    }
}
