using Godot;
using GodotUtils;
using System;
using System.Collections.Generic;

namespace Template.Inventory;

[SceneTree]
public partial class InventoryContainer : PanelContainer
{
    private Action<ClickType, Action, int> _onInput;

    private Action<int> _onPrePickup;
    private Action<int> _onPrePlace;
    private Action<int> _onPreStack;
    private Action<int> _onPreSwap;

    private Action<int> _onPostPickup;
    private Action<int> _onPostPlace;
    private Action<int> _onPostStack;
    private Action<int> _onPostSwap;

    private Holding _holding = new();
    private List<DummyItemContainer> _dummies = [];

    [OnInstantiate]
    private void Init(Inventory inventory)
    {
        AddItemContainers(inventory);
    }

    public override void _Input(InputEvent @event)
    {
        if (@event is InputEventMouseButton mouseBtn)
        {
            _holding.RightClick = mouseBtn.IsRightClickPressed();
            _holding.LeftClick = mouseBtn.IsLeftClickPressed();
        }
        else if (@event is InputEventKey key)
        {
            _holding.Shift = key.Keycode == Key.Shift && key.Pressed;
        }
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
                        cursorInventory.TakePartOfItemFrom(inventory, index, 0, item.Count);
                    }
                }
                else if (_holding.RightClick)
                {
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
        };

        _onPrePlace += index =>
        {
            itemFrame = cursorItemContainer.GetCurrentSpriteFrame();
        };

        _onPreSwap += index =>
        {
            itemFrame = itemContainers[index].GetCurrentSpriteFrame();
            cursorFrame = cursorItemContainer.GetCurrentSpriteFrame();
        };

        _onPreStack += index =>
        {
            itemFrame = itemContainers[index].GetCurrentSpriteFrame();
        };

        _onPostPickup += index =>
        {
            cursorItemContainer.SetCurrentSpriteFrame(itemFrame);

            CreateDummyItemContainer
            (
                DummyTarget.Cursor,
                cursorInventory.GetItem(0),
                itemContainers[index].GlobalPosition,
                itemFrame
            );
        };

        _onPostPlace += index =>
        {
            itemContainers[index].SetCurrentSpriteFrame(itemFrame);

            CreateDummyItemContainer
            (
                DummyTarget.Inventory,
                inventory.GetItem(index),
                cursorItemContainer.GlobalPosition,
                itemFrame,
                itemContainers[index]
            );
        };

        _onPostSwap += index =>
        {
            itemContainers[index].SetCurrentSpriteFrame(cursorFrame);
            cursorItemContainer.SetCurrentSpriteFrame(itemFrame);

            CreateDummyItemContainer
            (
                DummyTarget.Cursor,
                cursorInventory.GetItem(0),
                itemContainers[index].GlobalPosition,
                itemFrame
            );

            CreateDummyItemContainer
            (
                DummyTarget.Inventory,
                inventory.GetItem(index),
                cursorItemContainer.GlobalPosition,
                itemFrame,
                itemContainers[index]
            );
        };

        _onPostStack += index =>
        {
            itemContainers[index].SetCurrentSpriteFrame(itemFrame);
        };

        _onInput += (clickType, action, index) =>
        {
            // Ensure only the dummies for this session are active
            foreach (DummyItemContainer dummy in _dummies)
            {
                if (IsInstanceValid(dummy))
                {
                    dummy.QueueFree();
                }
            }

            _dummies.Clear();

            // Handle the click logic
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

    private void CreateDummyItemContainer(DummyTarget targetType, ItemStack itemStack, Vector2 position, int itemFrame, ItemContainer targetContainer = null)
    {
        DummyItemContainer dummy = DummyItemContainer.Instantiate(position, targetType, targetContainer);
        dummy.SetItem(itemStack);
        dummy.SetCurrentSpriteFrame(itemFrame);
        GetTree().Root.AddChildToCurrentScene(dummy);
        _dummies.Add(dummy);
    }

    private class Holding
    {
        public bool RightClick { get; set; }
        public bool LeftClick { get; set; }
        public bool Shift { get; set; }
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
}
