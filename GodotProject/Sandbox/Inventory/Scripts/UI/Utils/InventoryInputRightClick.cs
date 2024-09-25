using Godot;
using GodotUtils;

namespace Template.Inventory;

public class InventoryInputRightClick : InventoryInputHandler
{
    public override bool CheckInput(InputEventMouseButton mouseBtn)
    {
        return mouseBtn.IsRightClickPressed();
    }

    public override void HandleDiffType(InventorySlotContext context)
    {
        throw new System.NotImplementedException();
    }

    public override void HandlePickup(InventorySlotContext context)
    {
        throw new System.NotImplementedException();
    }

    public override void HandlePlace(InventorySlotContext context)
    {
        throw new System.NotImplementedException();
    }

    public override void HandleSameType(InventorySlotContext context)
    {
        throw new System.NotImplementedException();
    }
}
