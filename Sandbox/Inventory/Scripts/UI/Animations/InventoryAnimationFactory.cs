using System;
using System.Collections.Generic;

namespace Template.Inventory;

public class InventoryAnimationFactory
{
    private Dictionary<InventoryAction, InventoryAnimationBase> _animations = new()
    {
        { InventoryAction.Pickup, new PickupAnimation() },
        { InventoryAction.Place, new PlaceAnimation() },
        { InventoryAction.Stack, new StackAnimation() },
        { InventoryAction.Swap, new SwapAnimation() },
        { InventoryAction.Transfer, new TransferAnimation() }
    };

    public InventoryAnimationFactory(InventoryContext context, InventoryContainer container)
    {
        foreach (InventoryAnimationBase animation in _animations.Values)
        {
            animation.Initialise(context, container);
        }
    }

    public InventoryAnimationBase GetAnimation(InventoryAction actionType)
    {
        if (_animations.TryGetValue(actionType, out InventoryAnimationBase animation))
        {
            return animation;
        }

        throw new NotImplementedException($"No animation defined for {actionType}");
    }
}
