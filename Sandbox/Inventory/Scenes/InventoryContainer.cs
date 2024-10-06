using Godot;
using GodotUtils;
using System;

namespace Template.Inventory;

[SceneTree]
public partial class InventoryContainer : PanelContainer
{
    private Action<ClickType, Action, int> _onInput;

    private Action<ClickType, int> _onPrePickup, _onPrePlace, _onPreStack, _onPreSwap;
    private Action<ClickType, int> _onPostPickup, _onPostPlace, _onPostStack, _onPostSwap;

    private HoldingInput _holding = new();
    private CanvasLayer _ui;
    private InventoryVisualEffects _visualEffects;
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
        _visualEffects = new InventoryVisualEffects();

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

        InventoryVisualEffects.Context vfxContext = new(_ui, itemContainers, 
            cursorItemContainer, inventory, cursorInventory);

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
                        if (cursorInventory.HasItem(0) && !cursorInventory.GetItem(0).Material.Equals(inventory.GetItem(index).Material))
                        {
                            // Do nothing
                        }
                        else
                        {
                            _visualEffects.AnimateDragPickup(vfxContext, index);
                        }

                        cursorInventory.TakePartOfItemFrom(inventory, index, 0, item.Count);
                    }
                }
                else if (_holding.RightClick)
                {
                    // Only do animations when the cursor has a item and the inventory does
                    // not have an item. Otherwise too many animations gets too visually
                    // chaotic.
                    if (cursorInventory.HasItem(0) && !inventory.HasItem(index))
                    {
                        _visualEffects.AnimateDragPlace(vfxContext, index, GetGlobalMousePosition());
                    }

                    cursorInventory.MovePartOfItemTo(inventory, 0, index, 1);
                }
            };
        }

        inventory.OnItemChanged += (index, item) =>
        {
            itemContainers[index].SetItem(item);
        };

        int itemFrame = 0;
        int cursorFrame = 0;

        _onPrePickup += (clickType, index) =>
        {
            itemFrame = itemContainers[index].GetCurrentSpriteFrame();

            _visualEffects.AnimatePickup(vfxContext, index, itemFrame);
        };

        _onPostPickup += (clickType, index) =>
        {
            cursorItemContainer.HideSpriteAndCount(); // Needed for visual effects to work
            cursorItemContainer.SetCurrentSpriteFrame(itemFrame);

            // Ensure cursorItemContainer's position is in the correct position
            cursorItemContainer.Position = itemContainers[index].GlobalPosition;
        };

        _onPrePlace += (clickType, index) =>
        {
            itemFrame = cursorItemContainer.GetCurrentSpriteFrame();

            _visualEffects.AnimatePlace(vfxContext, index, itemFrame, GetGlobalMousePosition());
        };

        _onPostPlace += (clickType, index) =>
        {
            itemContainers[index].HideSpriteAndCount(); // Needed for visual effects to work
            itemContainers[index].SetCurrentSpriteFrame(itemFrame);
        };

        _onPreSwap += (clickType, index) =>
        {
            itemFrame = itemContainers[index].GetCurrentSpriteFrame();
            cursorFrame = cursorItemContainer.GetCurrentSpriteFrame();

            _visualEffects.AnimateSwap(vfxContext, index, itemFrame, GetGlobalMousePosition());
        };

        _onPostSwap += (clickType, index) =>
        {
            itemContainers[index].HideSpriteAndCount(); // Needed for visual effects to work
            cursorItemContainer.HideSpriteAndCount(); // Needed for visual effects to work

            itemContainers[index].SetCurrentSpriteFrame(cursorFrame);
            cursorItemContainer.SetCurrentSpriteFrame(itemFrame);

            // Ensure cursorItemContainer's position is in the correct position
            cursorItemContainer.Position = itemContainers[index].GlobalPosition;
        };

        _onPreStack += (clickType, index) =>
        {
            itemFrame = itemContainers[index].GetCurrentSpriteFrame();
        };

        _onPostStack += (clickType, index) =>
        {
            itemContainers[index].SetCurrentSpriteFrame(itemFrame);
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
                    _onPreStack(clickType, index);
                    _onInput(clickType, Action.Stack, index);
                    _onPostStack(clickType, index);
                }
                else
                {
                    // Swapping is disabled for right click operations
                    if (clickType == ClickType.Right)
                    {
                        return;
                    }

                    _onPreSwap(clickType, index);
                    _onInput(clickType, Action.Swap, index);
                    _onPostSwap(clickType, index);
                }
            }
            else
            {
                _onPrePlace(clickType, index);
                _onInput(clickType, Action.Place, index);
                _onPostPlace(clickType, index);
            }
        }
        else
        {
            if (inventory.HasItem(index))
            {
                _onPrePickup(clickType, index);
                _onInput(clickType, Action.Pickup, index);
                _onPostPickup(clickType, index);
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

    private enum ClickType
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
