using System;

namespace Template.Inventory;

public class InventoryActionFactory
{
    public InventoryActionBase GetAction(InventoryAction actionType)
    {
        return actionType switch
        {
            InventoryAction.DoubleClick => new PickupAllAction(),
            InventoryAction.Transfer => new TransferAction(),
            InventoryAction.Pickup => new PickupAction(),
            InventoryAction.Place => new PlaceAction(),
            InventoryAction.Stack => new StackAction(),
            InventoryAction.Swap => new SwapAction(),
            _ => throw new NotImplementedException($"No action defined for {actionType}")
        };
    }
}
