using Godot;
using GodotUtils;
using System;

namespace Template.Inventory;

public class InventoryInputHandler(InventoryInputDetector input)
{
    public event Action<ClickType, int> OnPrePickup, OnPrePlace, OnPreStack, OnPreSwap;
    public event Action<ClickType, int> OnPostPickup, OnPostPlace, OnPostStack, OnPostSwap;

    private Action<ClickType, Action, int> _onInput;

    public void RegisterInput(InventoryVFXContext context)
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
                TransferToOtherInventory(context);
            }
            else
            {
                Pickup(context.ClickType, context.Index);
            }
        }
    }

    private void TransferToOtherInventory(InputContext context)
    {
        Inventory otherInventory = Services.Get<InventorySandbox>().GetOtherInventory(context.Inventory);

        if (otherInventory.TryFindFirstEmptySlot(out int otherIndex))
        {
            context.Inventory.MoveItemTo(otherInventory, context.Index, otherIndex);
        }
    }

    private void Stack(ClickType clickType, int index)
    {
        OnPreStack?.Invoke(clickType, index);
        _onInput(clickType, Action.Stack, index);
        OnPostStack?.Invoke(clickType, index);
    }

    private void Swap(ClickType clickType, int index)
    {
        // Swapping is disabled for right click operations
        if (clickType == ClickType.Right)
        {
            return;
        }

        OnPreSwap?.Invoke(clickType, index);
        _onInput(clickType, Action.Swap, index);
        OnPostSwap?.Invoke(clickType, index);
    }

    private void Place(ClickType clickType, int index)
    {
        OnPrePlace?.Invoke(clickType, index);
        _onInput(clickType, Action.Place, index);
        OnPostPlace?.Invoke(clickType, index);
    }

    private void Pickup(ClickType clickType, int index)
    {
        OnPrePickup?.Invoke(clickType, index);
        _onInput(clickType, Action.Pickup, index);
        OnPostPickup?.Invoke(clickType, index);
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
        Swap
    }
}
