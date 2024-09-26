using Godot;

namespace Template.Inventory;

public abstract class InventoryInputHandler
{
    public void Handle(InventorySlotContext context)
    {
        Inventory inv = context.Inventory;
        CursorManager cursorManager = context.CursorManager;
        int index = context.Index;

        Item invItem = inv.GetItem(index);
        Item cursorItem = cursorManager.GetItem();

        bool hasInvItem = invItem != null;
        bool hasCursorItem = cursorItem != null;

        if (hasInvItem)
        {
            invItem.OnCountChanged += InvItemCountChanged;
        }

        if (hasCursorItem)
        {
            cursorItem.OnCountChanged += CursorItemCountChanged;
        }

        if (hasCursorItem)
        {
            if (hasInvItem)
            {
                if (cursorManager.ItemsAreOfSameType(invItem))
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
            if (hasInvItem)
            {
                HandlePickup(context);
            }
        }

        // Check again if inventory item is null
        if (inv.HasItem(index))
        {
            inv.GetItem(index).OnCountChanged -= InvItemCountChanged;
        }

        // Check again if cursor item is null
        if (cursorManager.HasItem())
        {
            cursorManager.GetItem().OnCountChanged -= CursorItemCountChanged;
        }

        void InvItemCountChanged(int count)
        {
            if (count <= 0)
            {
                inv.ClearItem(index);
            }
            else
            {
                context.InventoryManager.GetItemAndFrame(out Item item, out int frame);
                context.InventoryManager.SetItemAndFrame(item, frame);
            }
        }

        void CursorItemCountChanged(int count)
        {
            if (count <= 0)
            {
                cursorManager.ClearItem();
            }
            else
            {
                cursorManager.GetItemAndFrame(out Item item, out int frame);
                cursorManager.SetItem(item, context.ItemContainer.GlobalPosition, frame);
            }
        }
    }

    /// <summary>If the correct <paramref name="mouseBtn"/> input is provided then handle this input.</summary>
    public abstract bool HasInput(InputEventMouseButton mouseBtn);
    
    /// <summary>Both items are of the same type. The cursor and inventory slots are both guaranteed to have at least one item.</summary>
    public abstract void HandleSameType(InventorySlotContext context);
    
    /// <summary>Both items are of different types. The cursor and inventory slots are both guaranteed to have at least one item.</summary>
    public abstract void HandleDiffType(InventorySlotContext context);
    
    /// <summary>Placing an item from the cursor to the inventory. The cursor is guaranteed to have at least one item and there is no item in the inventory slot.</summary>
    public abstract void HandlePlace(InventorySlotContext context);
    
    /// <summary>Picking up an item from the inventory to the cursor. The inventory slot is guaranteed to have at least one item and there is no item in the cursor slot.</summary>
    public abstract void HandlePickup(InventorySlotContext context);
}
