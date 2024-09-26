using Godot;
using GodotUtils;

namespace Template.Inventory;

public class InventoryInputLeftClick : InventoryInputHandler
{
    public override bool HasInput(InputEventMouseButton mouseBtn)
    {
        return mouseBtn.IsLeftClickPressed();
    }

    // Stack the cursor item onto the inventory item
    public override void HandleSameType(InventorySlotContext context)
    {
        HandleSameTypeCommon(context, context.CursorManager.GetItem().Count);
    }

    public override void HandleDiffType(InventorySlotContext context)
    {
        base.HandleDiffType(context);
    }

    // Place the item from the cursor to the inventory
    public override void HandlePlace(InventorySlotContext context)
    {
        HandlePlaceCommon(context, context.CursorManager.GetItem().Count);
    }

    // Pickup the item from the inventory and put it on the cursor
    public override void HandlePickup(InventorySlotContext context)
    {
        HandlePickupCommon(context, context.Inventory.GetItem(context.Index).Count);
    }
}
