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

        void InvItemCountChanged(int count) => HandleInvItemCountChanged(context, count);
        void CursorItemCountChanged(int count) => HandleCursorItemCountChanged(context, count);
    }

    /// <summary>If the correct <paramref name="mouseBtn"/> input is provided then handle this input.</summary>
    public abstract bool HasInput(InputEventMouseButton mouseBtn);

    /// <summary>Both items are of the same type. The cursor and inventory slots are both guaranteed to have at least one item.</summary>
    public abstract void HandleSameType(InventorySlotContext context);

    /// <summary>Both items are of different types. The cursor and inventory slots are both guaranteed to have at least one item.</summary>
    public virtual void HandleDiffType(InventorySlotContext context)
    {
        // Get the cursor and inventory items
        context.CursorManager.GetItemAndFrame(out Item cursorItem, out int cursorItemFrame);
        context.InventoryManager.GetItemAndFrame(out Item invItem, out int invSpriteFrame);

        // Set the inv item with the cursor item
        context.InventoryManager.SetItemAndFrame(cursorItem, cursorItemFrame);

        // Set the cursor item with the inv item
        context.CursorManager.SetItem(invItem, context.ItemContainer.GlobalPosition, invSpriteFrame);
    }

    /// <summary>Placing an item from the cursor to the inventory. The cursor is guaranteed to have at least one item and there is no item in the inventory slot.</summary>
    public abstract void HandlePlace(InventorySlotContext context);

    /// <summary>Picking up an item from the inventory to the cursor. The inventory slot is guaranteed to have at least one item and there is no item in the cursor slot.</summary>
    public abstract void HandlePickup(InventorySlotContext context);

    protected void HandleSameTypeCommon(InventorySlotContext context, int amount)
    {
        // Increase the inventory item count by one
        context.Inventory.GetItem(context.Index).AddCount(amount);

        // Reduce the cursor item count by one
        context.CursorManager.GetItem().RemoveCount(amount);
    }

    protected void HandlePlaceCommon(InventorySlotContext context, int count)
    {
        context.CursorManager.GetItemAndFrame(out Item cursorItem, out int cursorItemFrame);

        // Create a new item with the specified count
        Item newItem = new(cursorItem);
        newItem.SetCount(count);

        // Reduce the cursor item count by the specified count
        cursorItem.RemoveCount(count);

        // Set the inventory item
        context.InventoryManager.SetItemAndFrame(newItem, cursorItemFrame);
    }

    protected void HandlePickupCommon(InventorySlotContext context, int count)
    {
        context.InventoryManager.GetItemAndFrame(out Item invItem, out int invSpriteFrame);

        // Create a new item with the specified count
        Item newItem = new(invItem);
        newItem.SetCount(count);

        // Reduce the inventory item count by the specified count
        invItem.RemoveCount(count);

        // Set the cursor item
        context.CursorManager.SetItem(newItem, context.ItemContainer.GlobalPosition, invSpriteFrame);
    }

    private void HandleInvItemCountChanged(InventorySlotContext context, int count)
    {
        if (count <= 0)
        {
            context.Inventory.ClearItem(context.Index);
        }
        else
        {
            context.InventoryManager.GetItemAndFrame(out Item item, out int frame);
            context.InventoryManager.SetItemAndFrame(item, frame);
        }
    }

    private void HandleCursorItemCountChanged(InventorySlotContext context, int count)
    {
        CursorManager cursorManager = context.CursorManager;
        Inventory cursorInventory = cursorManager.Inventory;

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
}
