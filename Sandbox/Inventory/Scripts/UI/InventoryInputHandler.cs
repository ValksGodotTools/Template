using Godot;
using GodotUtils;
using System;
using System.Collections.Generic;

namespace Template.Inventory;

public class InventoryInputHandler
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

    private Action<MouseButton, InventoryAction, int> _onInput;

    private InventoryInputDetector _input;
    private InventoryContainer _invContainerPlayer;
    private Inventory _invPlayer;
    private InventoryVFXContext _context;
    private (int, ItemStack) _itemUnderCursor;
    private Action _hotbarInputs;

    public InventoryInputHandler(int columns, InventoryInputDetector input, InventoryVFXContext context)
    {
        InventorySandbox sandbox = Services.Get<InventorySandbox>();

        _input = input;
        _context = context;
        _invContainerPlayer = sandbox.GetPlayerInventory();
        _invPlayer = _invContainerPlayer.Inventory;

        StringName[] hotbarActions = 
        [
            InputActions.Hotbar1,
            InputActions.Hotbar2,
            InputActions.Hotbar3,
            InputActions.Hotbar4,
            InputActions.Hotbar5,
            InputActions.Hotbar6,
            InputActions.Hotbar7,
            InputActions.Hotbar8,
            InputActions.Hotbar9,
            InputActions.Hotbar10,
            InputActions.Hotbar11,
            InputActions.Hotbar12
        ];

        for (int i = 0; i < columns; i++)
        {
            int index = i; // capture i

            _hotbarInputs += () =>
            {
                if (Input.IsActionJustPressed(hotbarActions[index]))
                {
                    int hotbarSlot = _invContainerPlayer.GetHotbarSlot(index);
                    _context.Inventory.MovePartOfItemTo(_invPlayer, _itemUnderCursor.Item1, hotbarSlot, _itemUnderCursor.Item2.Count);
                }
            };
        }
    }

    public void Update()
    {
        if (_itemUnderCursor.Item2 != null)
        {
            _hotbarInputs();
        }
    }

    public void RegisterInput(InventoryContainer container)
    {
        Inventory inventory = _context.Inventory;
        Inventory cursorInventory = _context.CursorInventory;

        _onInput += (mouseBtn, action, index) =>
        {
            if (mouseBtn == MouseButton.Left)
            {
                switch (action)
                {
                    case InventoryAction.Pickup:
                        _context.CursorInventory.TakeItemFrom(_context.Inventory, index, 0);
                        break;
                    case InventoryAction.Place:
                    case InventoryAction.Swap:
                    case InventoryAction.Stack:
                        cursorInventory.MoveItemTo(inventory, 0, index);
                        break;
                    case InventoryAction.Transfer:
                        LeftClickTransfer(container, index);
                        break;
                    case InventoryAction.DoubleClick:
                        DoubleClickPickupAll(cursorInventory, inventory, container, index);
                        break;
                }
            }
            else if (mouseBtn == MouseButton.Right)
            {
                switch (action)
                {
                    case InventoryAction.Pickup:
                        RightClickPickup(index);
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

    public void HandleGuiInput(InventoryContainer container, InputEvent @event, int index)
    {
        if (@event is InputEventMouseButton mouseButton)
        {
            if (mouseButton.DoubleClick)
            {
                if (mouseButton.ButtonIndex == MouseButton.Left)
                {
                    _onInput?.Invoke(MouseButton.Left, InventoryAction.DoubleClick, index);
                }
                else if (mouseButton.ButtonIndex == MouseButton.Right)
                {
                    _onInput?.Invoke(MouseButton.Right, InventoryAction.DoubleClick, index);
                }
            }
            else if (mouseButton.IsLeftClickJustPressed())
            {
                HandleClick(container, new InputContext(_context.Inventory, _context.CursorInventory, MouseButton.Left, index));
            }
            else if (mouseButton.IsRightClickJustPressed())
            {
                HandleClick(container, new InputContext(_context.Inventory, _context.CursorInventory, MouseButton.Right, index));
            }
        }
    }

    public void HandleMouseEntered(InventoryContainer container, InventoryVFXManager vfxManager, int index, Vector2 mousePos)
    {
        _itemUnderCursor = (index, _context.Inventory.GetItem(index));

        if (_input.HoldingLeftClick)
        {
            if (_itemUnderCursor.Item2 != null)
            {
                if (_input.HoldingShift)
                {
                    LeftClickTransfer(container, index);
                }
                else
                {
                    vfxManager.AnimateDragPickup(_context, index);
                    _context.CursorInventory.TakePartOfItemFrom(_context.Inventory, index, 0, _itemUnderCursor.Item2.Count);
                }
            }
        }
        else if (_input.HoldingRightClick)
        {
            vfxManager.AnimateDragPlace(_context, index, mousePos);
            _context.CursorInventory.MovePartOfItemTo(_context.Inventory, 0, index, 1);
        }
    }

    public void HandleMouseExited()
    {
        _itemUnderCursor.Item1 = -1;
        _itemUnderCursor.Item2 = null;
    }

    private void DoubleClickPickupAll(Inventory cursorInventory, Inventory inventory, InventoryContainer container, int index)
    {
        Material? material = cursorInventory.GetItem(0)?.Material ?? inventory.GetItem(index)?.Material;

        if (material != null)
        {
            InventoryContainer otherInvContainer = Services.Get<InventorySandbox>().GetOtherInventory(container);
            
            Dictionary<InventoryContainer, List<(int, ItemStack)>> items = [];
            items[container] = [];
            items[otherInvContainer] = [];

            foreach ((int i, ItemStack item) in inventory.GetItems())
            {
                if (item.Material.Equals(material))
                {
                    items[container].Add((i, item));
                }
            }

            foreach ((int i, ItemStack item) in otherInvContainer.Inventory.GetItems())
            {
                if (item.Material.Equals(material))
                {
                    items[otherInvContainer].Add((i, item));
                }
            }

            int sameItemCount = items[container].Count + items[otherInvContainer].Count;

            if (sameItemCount > 1)
            {
                foreach ((int i, ItemStack item) in items[container])
                {
                    // Do not animate index under cursor
                    if (i == index)
                        continue;

                    OnPrePickup?.Invoke(container, i);
                    cursorInventory.TakeItemFrom(inventory, i, 0);
                    OnPostPickup?.Invoke(container, i);
                }

                foreach ((int i, ItemStack item) in items[otherInvContainer])
                {
                    OnPrePickup?.Invoke(otherInvContainer, i);
                    cursorInventory.TakeItemFrom(otherInvContainer.Inventory, i, 0);
                    OnPostPickup?.Invoke(otherInvContainer, i);
                }
            }
        }
    }

    private void LeftClickTransfer(InventoryContainer container, int index)
    {
        InventoryContainer otherInventoryContainer = Services.Get<InventorySandbox>().GetOtherInventory(container);
        Inventory otherInventory = otherInventoryContainer.Inventory;

        if (otherInventory.TryFindFirstSameType(_context.Inventory.GetItem(index).Material, out int stackIndex))
        {
            Transfer(true, otherInventoryContainer, otherInventory, index, stackIndex);
        }
        else if (otherInventory.TryFindFirstEmptySlot(out int otherIndex))
        {
            Transfer(false, otherInventoryContainer, otherInventory, index, otherIndex);
        }
    }

    private void Transfer(bool areSameType, InventoryContainer otherInventoryContainer, Inventory otherInventory, int index, int otherIndex)
    {
        ItemContainer targetItemContainer = otherInventoryContainer.ItemContainers[otherIndex];

        TransferEventArgs args = new(areSameType, index, targetItemContainer);

        OnPreTransfer?.Invoke(args);

        _context.Inventory.MoveItemTo(otherInventory, index, otherIndex);

        OnPostTransfer?.Invoke(args);
    }

    private void RightClickPickup(int index)
    {
        Inventory cursorInventory = _context.CursorInventory;
        Inventory inventory = _context.Inventory;

        int halfItemCount = inventory.GetItem(index).Count / 2;

        if (_input.HoldingShift && halfItemCount != 0)
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
            Place(context.MouseButton, context.Index);
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
            Stack(context.MouseButton, index);
        }
        else
        {
            Swap(context.MouseButton, index);
        }
    }

    private void CursorHasNoItem(InventoryContainer container, InputContext context)
    {
        if (context.Inventory.HasItem(context.Index))
        {
            if (_input.HoldingShift && context.MouseButton == MouseButton.Left)
            {
                TransferToOtherInventory(context.MouseButton, context.Index);
            }
            else
            {
                Pickup(container, context.MouseButton, context.Index);
            }
        }
    }

    private void TransferToOtherInventory(MouseButton mouseBtn, int index)
    {
        _onInput(mouseBtn, InventoryAction.Transfer, index);
    }

    private void Stack(MouseButton mouseBtn, int index)
    {
        OnPreStack?.Invoke(index);
        _onInput(mouseBtn, InventoryAction.Stack, index);
        OnPostStack?.Invoke(index);
    }

    private void Swap(MouseButton mouseBtn, int index)
    {
        // Swapping is disabled for right click operations
        if (mouseBtn == MouseButton.Right)
        {
            return;
        }

        OnPreSwap?.Invoke(index);
        _onInput(mouseBtn, InventoryAction.Swap, index);
        OnPostSwap?.Invoke(index);
    }

    private void Place(MouseButton mouseBtn, int index)
    {
        OnPrePlace?.Invoke(index);
        _onInput(mouseBtn, InventoryAction.Place, index);
        OnPostPlace?.Invoke(index);
    }

    private void Pickup(InventoryContainer container, MouseButton mouseBtn, int index)
    {
        OnPrePickup?.Invoke(container, index);
        _onInput(mouseBtn, InventoryAction.Pickup, index);
        OnPostPickup?.Invoke(container, index);
    }

    private class InputContext(Inventory inventory, Inventory cursorInventory, MouseButton mouseBtn, int index)
    {
        public Inventory Inventory { get; } = inventory;
        public Inventory CursorInventory { get; } = cursorInventory;
        public MouseButton MouseButton { get; } = mouseBtn;
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
