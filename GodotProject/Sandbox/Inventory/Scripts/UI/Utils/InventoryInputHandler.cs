using Godot;

namespace Template.Inventory;

public abstract class InventoryInputHandler
{
    public void Handle(InventorySlotContext context)
    {
        Inventory inv = context.Inventory;
        CursorManager cursorManager = context.CursorManager;
        int index = context.Index;

        if (cursorManager.HasItem())
        {
            if (inv.HasItem(index))
            {
                if (cursorManager.ItemsAreOfSameType(inv.GetItem(index)))
                {
                    HandleSameType(context);
                }
                else
                {
                    HandleDiffType(context);
                }
            }
            else
            {
                HandlePlace(context);
            }
        }
        else
        {
            if (inv.HasItem(index))
            {
                HandlePickup(context);
            }
        }
    }

    public abstract bool CheckInput(InputEventMouseButton mouseBtn);
    public abstract void HandleSameType(InventorySlotContext context);
    public abstract void HandleDiffType(InventorySlotContext context);
    public abstract void HandlePlace(InventorySlotContext context);
    public abstract void HandlePickup(InventorySlotContext context);
}
