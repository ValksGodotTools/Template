﻿using Godot;
using GodotUtils;
using System;

namespace Template.Inventory;

public class InventoryInputHandler(InventoryInputDetector input)
{
    public event Action<int>
        OnPrePickup,
        OnPrePlace,
        OnPreStack,
        OnPreSwap,
        OnPostPickup,
        OnPostPlace,
        OnPostStack,
        OnPostSwap;

    public event Action<TransferEventArgs> OnPreTransfer;

    private Action<ClickType, Action, int> _onInput;

    public void RegisterInput(InventoryContainer container, InventoryVFXContext context)
    {
        Inventory inventory = context.Inventory;
        Inventory cursorInventory = context.CursorInventory;

        _onInput += (clickType, action, index) =>
        {
            if (clickType == ClickType.Left)
            {
                switch (action)
                {
                    case Action.Pickup:
                        context.CursorInventory.TakeItemFrom(context.Inventory, index, 0);
                        break;
                    case Action.Place:
                    case Action.Swap:
                    case Action.Stack:
                        cursorInventory.MoveItemTo(inventory, 0, index);
                        break;
                    case Action.Transfer:
                        LeftClickTransfer(container, context, index);
                        break;
                }
            }
            else if (clickType == ClickType.Right)
            {
                switch (action)
                {
                    case Action.Pickup:
                        RightClickPickup(context, index);
                        break;
                    case Action.Place:
                    case Action.Swap:
                    case Action.Stack:
                        cursorInventory.MovePartOfItemTo(inventory, 0, index, 1);
                        break;
                }
            }
        };
    }

    public void HandleGuiInput(InputEvent @event, InventoryVFXContext context, int index)
    {
        if (@event is InputEventMouseButton mouseButton)
        {
            if (mouseButton.IsLeftClickJustPressed())
            {
                HandleClick(new InputContext(context.Inventory, context.CursorInventory, ClickType.Left, index));
            }
            else if (mouseButton.IsRightClickJustPressed())
            {
                HandleClick(new InputContext(context.Inventory, context.CursorInventory, ClickType.Right, index));
            }
        }
    }

    public void HandleMouseEntered(InventoryVFXContext context, InventoryVFXManager vfxManager, int index, Vector2 mousePos)
    {
        if (input.HoldingLeftClick)
        {
            ItemStack item = context.Inventory.GetItem(index);

            if (item != null)
            {
                vfxManager.AnimateDragPickup(context, index);
                context.CursorInventory.TakePartOfItemFrom(context.Inventory, index, 0, item.Count);
            }
        }
        else if (input.HoldingRightClick)
        {
            vfxManager.AnimateDragPlace(context, index, mousePos);
            context.CursorInventory.MovePartOfItemTo(context.Inventory, 0, index, 1);
        }
    }

    private void LeftClickTransfer(InventoryContainer container, InventoryVFXContext context, int index)
    {
        InventoryContainer otherInventoryContainer = Services.Get<InventorySandbox>().GetOtherInventory(container);
        Inventory otherInventory = otherInventoryContainer.Inventory;

        if (otherInventory.TryFindFirstEmptySlot(out int otherIndex))
        {
            Vector2 targetPos = otherInventoryContainer.ItemContainers[otherIndex].GlobalPosition;
            OnPreTransfer?.Invoke(new TransferEventArgs(index, targetPos));

            context.Inventory.MoveItemTo(otherInventory, index, otherIndex);
        }
    }

    private void RightClickPickup(InventoryVFXContext context, int index)
    {
        Inventory cursorInventory = context.CursorInventory;
        Inventory inventory = context.Inventory;

        int halfItemCount = inventory.GetItem(index).Count / 2;

        if (input.HoldingShift && halfItemCount != 0)
        {
            cursorInventory.TakePartOfItemFrom(inventory, index, 0, halfItemCount);
        }
        else
        {
            cursorInventory.TakePartOfItemFrom(inventory, index, 0, 1);
        }
    }

    private void HandleClick(InputContext context)
    {
        if (context.CursorInventory.HasItem(0))
        {
            CursorHasItem(context);
        }
        else
        {
            CursorHasNoItem(context);
        }
    }

    private void CursorHasItem(InputContext context)
    {
        if (context.Inventory.HasItem(context.Index))
        {
            CursorAndInventoryHaveItems(context);
        }
        else
        {
            // Cursor has item but inv slot does not
            Place(context.ClickType, context.Index);
        }
    }

    private void CursorAndInventoryHaveItems(InputContext context)
    {
        int index = context.Index;

        Material cursorMaterial = context.CursorInventory.GetItem(0).Material;
        Material invMaterial = context.Inventory.GetItem(index).Material;

        // The cursor item and inventory item are of the same type
        if (cursorMaterial.Equals(invMaterial))
        {
            Stack(context.ClickType, index);
        }
        else
        {
            Swap(context.ClickType, index);
        }
    }

    private void CursorHasNoItem(InputContext context)
    {
        if (context.Inventory.HasItem(context.Index))
        {
            if (input.HoldingShift)
            {
                TransferToOtherInventory(context.ClickType, context.Index);
            }
            else
            {
                Pickup(context.ClickType, context.Index);
            }
        }
    }

    private void TransferToOtherInventory(ClickType clickType, int index)
    {
        _onInput(clickType, Action.Transfer, index);
    }

    private void Stack(ClickType clickType, int index)
    {
        OnPreStack?.Invoke(index);
        _onInput(clickType, Action.Stack, index);
        OnPostStack?.Invoke(index);
    }

    private void Swap(ClickType clickType, int index)
    {
        // Swapping is disabled for right click operations
        if (clickType == ClickType.Right)
        {
            return;
        }

        OnPreSwap?.Invoke(index);
        _onInput(clickType, Action.Swap, index);
        OnPostSwap?.Invoke(index);
    }

    private void Place(ClickType clickType, int index)
    {
        OnPrePlace?.Invoke(index);
        _onInput(clickType, Action.Place, index);
        OnPostPlace?.Invoke(index);
    }

    private void Pickup(ClickType clickType, int index)
    {
        OnPrePickup?.Invoke(index);
        _onInput(clickType, Action.Pickup, index);
        OnPostPickup?.Invoke(index);
    }

    public enum ClickType
    {
        Left,
        Right
    }

    private class InputContext(Inventory inventory, Inventory cursorInventory, ClickType clickType, int index)
    {
        public Inventory Inventory { get; } = inventory;
        public Inventory CursorInventory { get; } = cursorInventory;
        public ClickType ClickType { get; } = clickType;
        public int Index { get; } = index;
    }

    private enum Action
    {
        Pickup,
        Place,
        Stack,
        Swap,
        Transfer
    }
}

public class TransferEventArgs(int fromIndex, Vector2 targetPos)
{
    public int FromIndex { get; } = fromIndex;
    public Vector2 TargetPos { get; } = targetPos;
}
