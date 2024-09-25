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

    /// <summary>If the correct <paramref name="mouseBtn"/> input is provided then handle this input.</summary>
    public abstract bool CheckInput(InputEventMouseButton mouseBtn);
    
    /// <summary>Both items are of the same type. The cursor and inventory slots are both guaranteed to have at least one item.</summary>
    public abstract void HandleSameType(InventorySlotContext context);
    
    /// <summary>Both items are of different types. The cursor and inventory slots are both guaranteed to have at least one item.</summary>
    public abstract void HandleDiffType(InventorySlotContext context);
    
    /// <summary>Placing an item from the cursor to the inventory. The cursor is guaranteed to have at least one item and there is no item in the inventory slot.</summary>
    public abstract void HandlePlace(InventorySlotContext context);
    
    /// <summary>Picking up an item from the inventory to the cursor. The inventory slot is guaranteed to have at least one item and there is no item in the cursor slot.</summary>
    public abstract void HandlePickup(InventorySlotContext context);
}
