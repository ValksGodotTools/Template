using Godot;
using GodotUtils;
using System;

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

    private Holding _holding;

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

        _onPostPickup += index =>
        {
            cursorItemContainer.SetCurrentSpriteFrame(itemFrame);

            // Ensure cursorItemContainer's position is in the correct position
            cursorItemContainer.Position = itemContainers[index].GlobalPosition;
            cursorItemContainer.ResetSmoothFactor();
        };

        _onPrePlace += index =>
        {
            itemFrame = cursorItemContainer.GetCurrentSpriteFrame();
        };

        _onPostPlace += index =>
        {
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
            cursorItemContainer.ResetSmoothFactor();
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

public enum ClickType
{
    Left,
    Right
}

public enum Action
{
    Pickup,
    Place,
    Stack,
    Swap
}
