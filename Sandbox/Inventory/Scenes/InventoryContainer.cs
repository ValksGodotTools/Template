using Godot;
using GodotUtils;
using System;

namespace Template.Inventory;

[SceneTree]
public partial class InventoryContainer : PanelContainer
{
    private Action<ClickType, Action, int> _onInput;

    public event Action<ClickType, int> OnPrePickup, OnPrePlace, OnPreStack, OnPreSwap;
    public event Action<ClickType, int> OnPostPickup, OnPostPlace, OnPostStack, OnPostSwap;

    private HoldingInput _holding = new();
    private CanvasLayer _ui;
    private Inventory _inventory;

    [OnInstantiate]
    private void Init(Inventory inventory, int columns = 10)
    {
        GridContainer.Columns = columns;
        _inventory = inventory;
    }

    public override void _Ready()
    {
        _ui = GetTree().CurrentScene.GetNode<CanvasLayer>("%UI");

        AddItemContainers(_inventory);
    }

    public override void _Input(InputEvent @event)
    {
        _holding.InputUpdate(@event);
    }

    private void AddItemContainers(Inventory inventory)
    {
        ItemContainer[] itemContainers = new ItemContainer[inventory.GetItemSlotCount()];
        CursorItemContainer cursorItemContainer = Services.Get<CursorItemContainer>();
        Inventory cursorInventory = cursorItemContainer.Inventory;
        InventoryVFX inventoryVFX = new();

        InventoryVFXContext vfxContext = new(_ui, inventoryVFX, itemContainers, 
            cursorItemContainer, inventory, cursorInventory);

        InventoryVFXManager vfxManager = new();

        vfxManager.RegisterEvents(this, vfxContext);

        for (int i = 0; i < itemContainers.Length; i++)
        {
            ItemContainer itemContainer = AddItemContainer();
            itemContainer.SetItem(inventory.GetItem(i));
            itemContainers[i] = itemContainer;

            int index = i; // Capture i

            itemContainer.GuiInput += @event =>
            {
                if (@event is InputEventMouseButton mouseButton)
                {
                    if (mouseButton.IsLeftClickJustPressed())
                    {
                        HandleClick(ClickType.Left, index, cursorInventory, inventory);
                    }
                    else if (mouseButton.IsRightClickJustPressed())
                    {
                        HandleClick(ClickType.Right, index, cursorInventory, inventory);
                    }
                }
            };

            itemContainer.MouseEntered += () =>
            {
                if (_holding.LeftClick)
                {
                    ItemStack item = inventory.GetItem(index);

                    if (item != null)
                    {
                        vfxManager.AnimateDragPickup(vfxContext, index);
                        cursorInventory.TakePartOfItemFrom(inventory, index, 0, item.Count);
                    }
                }
                else if (_holding.RightClick)
                {
                    vfxManager.AnimateDragPlace(vfxContext, index, GetGlobalMousePosition());
                    cursorInventory.MovePartOfItemTo(inventory, 0, index, 1);
                }
            };
        }

        inventory.OnItemChanged += (index, item) =>
        {
            itemContainers[index].SetItem(item);
        };

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
                        cursorInventory.TakePartOfItemFrom(inventory, index, 0, 1);
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

    private ItemContainer AddItemContainer()
    {
        ItemContainer itemContainer = ItemContainer.Instantiate();
        GridContainer.AddChild(itemContainer);
        return itemContainer;
    }

    #region Class Helpers
    private class HoldingInput
    {
        public bool RightClick { get; private set; }
        public bool LeftClick { get; private set; }
        public bool Shift { get; private set; }

        public void InputUpdate(InputEvent @event)
        {
            if (@event is InputEventMouseButton mouseBtn)
            {
                RightClick = mouseBtn.IsRightClickPressed();
                LeftClick = mouseBtn.IsLeftClickPressed();
            }
            else if (@event is InputEventKey key)
            {
                Shift = key.Keycode == Key.Shift && key.Pressed;
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
    #endregion
}
