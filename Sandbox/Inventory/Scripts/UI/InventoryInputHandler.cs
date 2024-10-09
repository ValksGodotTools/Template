using Godot;
using GodotUtils;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Template.Inventory;

public class InventoryInputHandler(InventoryInputDetector input)
{
    public event Action<int>
        OnPrePlace,
        OnPreStack,
        OnPreSwap,
        OnPostPlace,
        OnPostStack,
        OnPostSwap;

    public event Action<InventoryContainer, int> OnPrePickup, OnPostPickup;

    public event Action<TransferEventArgs>
        OnPreTransfer,
        OnPostTransfer;

    private Action<ClickType, InventoryAction, int> _onInput;

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
                    case InventoryAction.Pickup:
                        context.CursorInventory.TakeItemFrom(context.Inventory, index, 0);
                        break;
                    case InventoryAction.Place:
                    case InventoryAction.Swap:
                    case InventoryAction.Stack:
                        cursorInventory.MoveItemTo(inventory, 0, index);
                        break;
                    case InventoryAction.Transfer:
                        LeftClickTransfer(container, context, index);
                        break;
                    case InventoryAction.DoubleClick:
                        DoubleClickPickupAll(cursorInventory, inventory, container, index);
                        break;
                }
            }
            else if (clickType == ClickType.Right)
            {
                switch (action)
                {
                    case InventoryAction.Pickup:
                        RightClickPickup(context, index);
                        break;
                    case InventoryAction.Place:
                    case InventoryAction.Swap:
                    case InventoryAction.Stack:
                        cursorInventory.MovePartOfItemTo(inventory, 0, index, 1);
                        break;
                }
            }
        };
    }

    public void HandleGuiInput(InventoryContainer container, InputEvent @event, InventoryVFXContext context, int index)
    {
        if (@event is InputEventMouseButton mouseButton)
        {
            if (mouseButton.IsLeftClickJustPressed())
            {
                HandleClick(container, new InputContext(context.Inventory, context.CursorInventory, ClickType.Left, index));
            }
            else if (mouseButton.IsRightClickJustPressed())
            {
                HandleClick(container, new InputContext(context.Inventory, context.CursorInventory, ClickType.Right, index));
            }

            if (mouseButton.DoubleClick)
            {
                if (mouseButton.ButtonIndex == MouseButton.Left)
                {
                    _onInput?.Invoke(ClickType.Left, InventoryAction.DoubleClick, index);
                }
                else if (mouseButton.ButtonIndex == MouseButton.Right)
                {
                    _onInput?.Invoke(ClickType.Right, InventoryAction.DoubleClick, index);
                }
            }
        }
    }

    public void HandleMouseEntered(InventoryContainer container, InventoryVFXContext context, InventoryVFXManager vfxManager, int index, Vector2 mousePos)
    {
        if (input.HoldingLeftClick)
        {
            ItemStack item = context.Inventory.GetItem(index);

            if (item != null)
            {
                if (input.HoldingShift)
                {
                    LeftClickTransfer(container, context, index);
                }
                else
                {
                    vfxManager.AnimateDragPickup(context, index);
                    context.CursorInventory.TakePartOfItemFrom(context.Inventory, index, 0, item.Count);
                }
            }
        }
        else if (input.HoldingRightClick)
        {
            vfxManager.AnimateDragPlace(context, index, mousePos);
            context.CursorInventory.MovePartOfItemTo(context.Inventory, 0, index, 1);
        }
    }

    private void DoubleClickPickupAll(Inventory cursorInventory, Inventory inventory, InventoryContainer container, int index)
    {
        Material? material = cursorInventory.GetItem(0)?.Material ?? inventory.GetItem(index)?.Material;

        if (material != null)
        {
            foreach ((int i, ItemStack item) in inventory.GetItems())
            {
                if (item.Material.Equals(material))
                {
                    OnPrePickup?.Invoke(container, i);
                    cursorInventory.TakeItemFrom(inventory, i, 0);
                    OnPostPickup?.Invoke(container, i);
                }
            }

            InventoryContainer otherInvContainer = Services.Get<InventorySandbox>().GetOtherInventory(container);

            foreach ((int i, ItemStack item) in otherInvContainer.Inventory.GetItems())
            {
                if (item.Material.Equals(material))
                {
                    OnPrePickup?.Invoke(otherInvContainer, i);
                    cursorInventory.TakeItemFrom(otherInvContainer.Inventory, i, 0);
                    OnPostPickup?.Invoke(otherInvContainer, i);
                }
            }
        }
    }

    private void LeftClickTransfer(InventoryContainer container, InventoryVFXContext context, int index)
    {
        InventoryContainer otherInventoryContainer = Services.Get<InventorySandbox>().GetOtherInventory(container);
        Inventory otherInventory = otherInventoryContainer.Inventory;

        if (otherInventory.TryFindFirstSameType(context.Inventory.GetItem(index).Material, out int stackIndex))
        {
            Transfer(true, otherInventoryContainer, otherInventory, context, index, stackIndex);
        }
        else if (otherInventory.TryFindFirstEmptySlot(out int otherIndex))
        {
            Transfer(false, otherInventoryContainer, otherInventory, context, index, otherIndex);
        }
    }

    private void Transfer(bool areSameType, InventoryContainer otherInventoryContainer, Inventory otherInventory, InventoryVFXContext context, int index, int otherIndex)
    {
        ItemContainer targetItemContainer = otherInventoryContainer.ItemContainers[otherIndex];

        TransferEventArgs args = new(areSameType, index, targetItemContainer);

        OnPreTransfer?.Invoke(args);

        context.Inventory.MoveItemTo(otherInventory, index, otherIndex);

        OnPostTransfer?.Invoke(args);
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

    private void HandleClick(InventoryContainer container, InputContext context)
    {
        if (context.CursorInventory.TryGetItem(0, out ItemStack cursorItem))
        {
            CursorHasItem(context, cursorItem);
        }
        else
        {
            CursorHasNoItem(container, context);
        }
    }

    private void CursorHasItem(InputContext context, ItemStack cursorItem)
    {
        if (context.Inventory.TryGetItem(context.Index, out ItemStack invItem))
        {
            CursorAndInventoryHaveItems(context, invItem, cursorItem);
        }
        else
        {
            // Cursor has item but inv slot does not
            Place(context.ClickType, context.Index);
        }
    }

    private void CursorAndInventoryHaveItems(InputContext context, ItemStack invItem, ItemStack cursorItem)
    {
        int index = context.Index;

        Material cursorMaterial = cursorItem.Material;
        Material invMaterial = invItem.Material;

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

    private void CursorHasNoItem(InventoryContainer container, InputContext context)
    {
        if (context.Inventory.HasItem(context.Index))
        {
            if (input.HoldingShift)
            {
                TransferToOtherInventory(context.ClickType, context.Index);
            }
            else
            {
                Pickup(container, context.ClickType, context.Index);
            }
        }
    }

    private void TransferToOtherInventory(ClickType clickType, int index)
    {
        _onInput(clickType, InventoryAction.Transfer, index);
    }

    private void Stack(ClickType clickType, int index)
    {
        OnPreStack?.Invoke(index);
        _onInput(clickType, InventoryAction.Stack, index);
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
        _onInput(clickType, InventoryAction.Swap, index);
        OnPostSwap?.Invoke(index);
    }

    private void Place(ClickType clickType, int index)
    {
        OnPrePlace?.Invoke(index);
        _onInput(clickType, InventoryAction.Place, index);
        OnPostPlace?.Invoke(index);
    }

    private void Pickup(InventoryContainer container, ClickType clickType, int index)
    {
        OnPrePickup?.Invoke(container, index);
        _onInput(clickType, InventoryAction.Pickup, index);
        OnPostPickup?.Invoke(container, index);
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

    private enum InventoryAction
    {
        Pickup,
        Place,
        Stack,
        Swap,
        Transfer,
        DoubleClick
    }
}

public class TransferEventArgs(bool stacking, int fromIndex, ItemContainer targetItemContainer)
{
    public bool AreSameType { get; } = stacking;
    public int FromIndex { get; } = fromIndex;
    public ItemContainer TargetItemContainer { get; } = targetItemContainer;
}
