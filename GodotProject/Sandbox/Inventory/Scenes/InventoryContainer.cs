using Godot;
using GodotUtils;
using System;

namespace Template.Inventory;

[SceneTree]
public partial class InventoryContainer : PanelContainer
{
    public event Action<ClickType, Action, int> OnInput;

    [OnInstantiate]
    private void Init(Inventory inventory)
    {
        AddItemContainers(inventory);
    }

    private void AddItemContainers(Inventory inventory)
    {
        ItemContainer[] itemContainers = new ItemContainer[inventory.GetInventorySize()];
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

        OnInput += (clickType, action, index) =>
        {
            if (clickType == ClickType.Left)
            {
                if (action == Action.Pickup)
                {
                    cursorInventory.TakeItemFrom(inventory, index, 0);
                }
                else if (action == Action.Place)
                {
                    cursorInventory.MoveItemTo(inventory, 0, index);
                }
                else if (action == Action.Swap)
                {
                    cursorInventory.MoveItemTo(inventory, 0, index);
                }
                else if (action == Action.Stack)
                {
                    cursorInventory.MoveItemTo(inventory, 0, index);
                }
            }
            else if (clickType == ClickType.Right)
            {
                // TODO
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
                    OnInput?.Invoke(clickType, Action.Stack, index);
                }
                else
                {
                    OnInput?.Invoke(clickType, Action.Swap, index);
                }
            }
            else
            {
                OnInput?.Invoke(clickType, Action.Place, index);
            }
        }
        else
        {
            if (inventory.HasItem(index))
            {
                OnInput?.Invoke(clickType, Action.Pickup, index);
            }
        }
    }


    private ItemContainer AddItemContainer()
    {
        ItemContainer itemContainer = ItemContainer.Instantiate();
        GridContainer.AddChild(itemContainer);
        return itemContainer;
    }
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
