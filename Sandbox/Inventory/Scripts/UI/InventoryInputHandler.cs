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
                        cursorInventory.TakeItemFrom(inventory, index, 0);
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
                HandleClick(ClickType.Left, index, context.CursorInventory, context.Inventory);
            }
            else if (mouseButton.IsRightClickJustPressed())
            {
                HandleClick(ClickType.Right, index, context.CursorInventory, context.Inventory);
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

    private void HandleClick(ClickType clickType, int index, Inventory cursorInventory, Inventory inventory)
    {
        if (cursorInventory.HasItem(0))
        {
            if (inventory.HasItem(index))
            {
                if (cursorInventory.GetItem(0).Material.Equals(inventory.GetItem(index).Material))
                {
                    OnPreStack?.Invoke(clickType, index);
                    _onInput(clickType, Action.Stack, index);
                    OnPostStack?.Invoke(clickType, index);
                }
                else
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
            }
            else
            {
                OnPrePlace?.Invoke(clickType, index);
                _onInput(clickType, Action.Place, index);
                OnPostPlace?.Invoke(clickType, index);
            }
        }
        else
        {
            if (inventory.HasItem(index))
            {
                OnPrePickup?.Invoke(clickType, index);
                _onInput(clickType, Action.Pickup, index);
                OnPostPickup?.Invoke(clickType, index);
            }
        }
    }

    public enum ClickType
    {
        Left,
        Right
    }

    private enum Action
    {
        Pickup,
        Place,
        Stack,
        Swap
    }
}
