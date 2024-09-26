using Godot;
using System;

namespace Template.Inventory;

public abstract class InventoryInputHandler
{
    public void Handle(InventorySlotContext context)
    {
        Inventory inv = context.Inventory;
        CursorManager cursorManager = context.CursorManager;
        int index = context.Index;

        Inventory cursorInventory = cursorManager.Inventory;

        Item invItem = inv.GetItem(index);
        Item cursorItem = cursorInventory.GetItem(0);

        // We no longer have to worry about setting the visual properties of an item when
        // its count has been changed
        SubscribeToCountChanged(invItem, InvItemCountChanged);
        SubscribeToCountChanged(cursorItem, CursorItemCountChanged);

        if (cursorItem != null)
        {
            if (invItem != null)
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
        else if (invItem != null)
        {
            HandlePickup(context);
        }

        UnsubscribeFromCountChanged(inv.GetItem(index), InvItemCountChanged);
        UnsubscribeFromCountChanged(cursorInventory.GetItem(0), CursorItemCountChanged);

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
                cursorInventory.ClearItem(0);
            }
            else
            {
                cursorManager.GetItemAndFrame(out Item item, out int frame);
                cursorManager.SetItem(item, context.ItemContainer.GlobalPosition, frame);
            }
        }
    }

    private void SubscribeToCountChanged(Item item, Action<int> countChangedHandler)
    {
        if (item != null)
        {
            item.OnCountChanged += countChangedHandler;
        }
    }

    private void UnsubscribeFromCountChanged(Item item, Action<int> countChangedHandler)
    {
        if (item != null)
        {
            item.OnCountChanged -= countChangedHandler;
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
