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
        CursorInventory cursorInventory = cursorItemContainer.Inventory;

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
                    cursorInventory.TakeItemFrom(inventory, index);
                }
                else if (action == Action.Place)
                {
                    cursorInventory.MoveItemTo(inventory, index);
                }
            }
            else if (clickType == ClickType.Right)
            {

            }
        };
    }

    private void HandleClick(ClickType clickType, int index, CursorInventory cursorInventory, Inventory inventory)
    {
        if (cursorInventory.HasItem())
        {
            if (inventory.HasItem(index))
            {
                if (cursorInventory.GetItem().Equals(inventory.GetItem(index)))
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
