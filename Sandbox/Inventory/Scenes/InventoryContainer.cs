using Godot;
using GodotUtils;
using System;

namespace Template.Inventory;

[SceneTree]
public partial class InventoryContainer : PanelContainer
{
    private Action<ClickType, Action, int> _onInput;

    private Action<int> _onPrePickup, _onPrePlace, _onPreStack, _onPreSwap;
    private Action<int> _onPostPickup, _onPostPlace, _onPostStack, _onPostSwap;

    private HoldingInput _holding = new();
    private CanvasLayer _ui;

    [OnInstantiate]
    private void Init(Inventory inventory, int columns = 10)
    {
        GridContainer.Columns = columns;
        AddItemContainers(inventory);
    }

    public override void _Ready()
    {
        _ui = GetTree().CurrentScene.GetNode<CanvasLayer>("%UI");
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
                        // ------------------- UI VISUAL CODE -------------------
                        if (cursorInventory.HasItem(0) && !cursorInventory.GetItem(0).Material.Equals(inventory.GetItem(index).Material))
                        {
                            // Do nothing
                        }
                        else
                        {
                            AnimHelperItemContainer container = new AnimHelperItemContainer.Builder(AnimHelperItemContainer.Instantiate())
                                .SetInitialPositionForControl(itemContainers[index].GlobalPosition)
                                .SetTargetAsMouse()
                                .SetStartingLerp(0.3f) // Need to make animation quick
                                .SetItemAndFrame(inventory.GetItem(index), 0)
                                .SetCount(0) // Too much information on screen gets chaotic
                                .Build();

                            _ui.AddChild(container);

                            if (!cursorInventory.HasItem(0))
                            {
                                cursorItemContainer.HideSpriteAndCount();
                            }

                            container.OnReachedTarget += () =>
                            {
                                cursorItemContainer.ShowSpriteAndCount();
                            };
                        }
                        // ------------------- UI VISUAL CODE -------------------

                        cursorInventory.TakePartOfItemFrom(inventory, index, 0, item.Count);
                    }
                }
                else if (_holding.RightClick)
                {
                    // ------------------- UI VISUAL CODE -------------------
                    // Only do animations when the cursor has a item and the inventory does
                    // not have an item. Otherwise too many animations gets too visually
                    // chaotic.
                    if (cursorInventory.HasItem(0) && !inventory.HasItem(index))
                    {
                        // Place one of item from cursor to inventory slot
                        AnimHelperItemContainer container = new AnimHelperItemContainer.Builder(AnimHelperItemContainer.Instantiate())
                            .SetInitialPositionForNode2D(GetGlobalMousePosition())
                            .SetControlTarget(itemContainers[index].GlobalPosition)
                            .SetItemAndFrame(cursorInventory.GetItem(0), 0)
                            .SetCount(0) // Too much information on screen gets chaotic
                            .Build();

                        itemContainers[index].HideSpriteAndCount();

                        container.OnReachedTarget += () =>
                        {
                            itemContainers[index].ShowSpriteAndCount();
                        };

                        _ui.AddChild(container);
                    }
                    // ------------------- UI VISUAL CODE -------------------

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

        _onPrePickup += index =>
        {
            itemFrame = itemContainers[index].GetCurrentSpriteFrame();

            // ------------------- UI VISUAL CODE -------------------
            AnimHelperItemContainer container = new AnimHelperItemContainer.Builder(AnimHelperItemContainer.Instantiate())
                .SetInitialPositionForControl(itemContainers[index].GlobalPosition)
                .SetTargetAsMouse()
                .SetItemAndFrame(inventory.GetItem(index), itemFrame)
                .Build();

            container.OnReachedTarget += () =>
            {
                cursorItemContainer.ShowSpriteAndCount();
            };

            _ui.AddChild(container);
            // ------------------- UI VISUAL CODE -------------------
        };

        _onPostPickup += index =>
        {
            cursorItemContainer.HideSpriteAndCount();
            cursorItemContainer.SetCurrentSpriteFrame(itemFrame);

            // Ensure cursorItemContainer's position is in the correct position
            cursorItemContainer.Position = itemContainers[index].GlobalPosition;
        };

        _onPrePlace += index =>
        {
            itemFrame = cursorItemContainer.GetCurrentSpriteFrame();

            // ------------------- UI VISUAL CODE -------------------
            AnimHelperItemContainer container = new AnimHelperItemContainer.Builder(AnimHelperItemContainer.Instantiate())
                .SetInitialPositionForNode2D(GetGlobalMousePosition())
                .SetControlTarget(itemContainers[index].GlobalPosition)
                .SetItemAndFrame(cursorInventory.GetItem(0), itemFrame)
                .Build();

            container.OnReachedTarget += () =>
            {
                itemContainers[index].ShowSpriteAndCount();
            };

            _ui.AddChild(container);
            // ------------------- UI VISUAL CODE -------------------
        };

        _onPostPlace += index =>
        {
            itemContainers[index].HideSpriteAndCount();
            itemContainers[index].SetCurrentSpriteFrame(itemFrame);
        };

        _onPreSwap += index =>
        {
            itemFrame = itemContainers[index].GetCurrentSpriteFrame();
            cursorFrame = cursorItemContainer.GetCurrentSpriteFrame();
        };

        _onPostSwap += index =>
        {
            itemContainers[index].SetCurrentSpriteFrame(cursorFrame);
            cursorItemContainer.SetCurrentSpriteFrame(itemFrame);

            // Ensure cursorItemContainer's position is in the correct position
            cursorItemContainer.Position = itemContainers[index].GlobalPosition;
        };

        _onPreStack += index =>
        {
            itemFrame = itemContainers[index].GetCurrentSpriteFrame();
        };

        _onPostStack += index =>
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
                    _onPreStack(index);
                    _onInput(clickType, Action.Stack, index);
                    _onPostStack(index);
                }
                else
                {
                    // Swapping is disabled for right click operations
                    if (clickType == ClickType.Right)
                    {
                        return;
                    }

                    _onPreSwap(index);
                    _onInput(clickType, Action.Swap, index);
                    _onPostSwap(index);
                }
            }
            else
            {
                _onPrePlace(index);
                _onInput(clickType, Action.Place, index);
                _onPostPlace(index);
            }
        }
        else
        {
            if (inventory.HasItem(index))
            {
                _onPrePickup(index);
                _onInput(clickType, Action.Pickup, index);
                _onPostPickup(index);
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
