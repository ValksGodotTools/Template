using Godot;
using GodotUtils;
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

        CountChangedHandler countChanged = new();

        // We no longer have to worry about setting the visual properties of an item when
        // its count has been changed
        countChanged.Subscribe(invItem, InvItemCountChanged);
        countChanged.Subscribe(cursorItem, CursorItemCountChanged);

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

        countChanged.Unsubscribe(inv.GetItem(index), InvItemCountChanged);
        countChanged.Unsubscribe(cursorInventory.GetItem(0), CursorItemCountChanged);

        void InvItemCountChanged(int count) => countChanged.InvItem(context, count);
        void CursorItemCountChanged(int count) => countChanged.CursorItem(context, count);
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
        context.CursorManager.SetItemAndFrame(invItem, invSpriteFrame);
        context.CursorManager.SetPosition(context.ItemContainer.GlobalPosition);
    }

    /// <summary>Placing an item from the cursor to the inventory. The cursor is guaranteed to have at least one item and there is no item in the inventory slot.</summary>
    public abstract void HandlePlace(InventorySlotContext context);

    /// <summary>Picking up an item from the inventory to the cursor. The inventory slot is guaranteed to have at least one item and there is no item in the cursor slot.</summary>
    public abstract void HandlePickup(InventorySlotContext context);

    protected class InputCommon
    {
        public static void HandleSameType(InventorySlotContext context, int amount)
        {
            // Increase the inventory item count by one
            context.Inventory.GetItem(context.Index).AddCount(amount);

            // Reduce the cursor item count by one
            context.CursorManager.GetItem().RemoveCount(amount);
        }

        public static void HandlePlace(InventorySlotContext context, int count)
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

        public static void HandlePickup(InventorySlotContext context, int count)
        {
            context.InventoryManager.GetItemAndFrame(out Item invItem, out int invSpriteFrame);

            // Create a new item with the specified count
            Item newItem = new(invItem);
            newItem.SetCount(count);

            // Reduce the inventory item count by the specified count
            invItem.RemoveCount(count);

            // Set the cursor item
            context.CursorManager.SetItemAndFrame(newItem, invSpriteFrame);
            context.CursorManager.SetPosition(context.ItemContainer.GlobalPosition);
        }
    }

    private class CountChangedHandler
    {
        public void InvItem(InventorySlotContext context, int count)
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

        public void CursorItem(InventorySlotContext context, int count)
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
                cursorManager.SetItemAndFrame(item, frame);
            }
        }

        public void Subscribe(Item item, Action<int> countChangedHandler)
        {
            if (item != null)
            {
                item.OnCountChanged += countChangedHandler;
            }
        }

        public void Unsubscribe(Item item, Action<int> countChangedHandler)
        {
            if (item != null)
            {
                item.OnCountChanged -= countChangedHandler;
            }
        }
    }
}
