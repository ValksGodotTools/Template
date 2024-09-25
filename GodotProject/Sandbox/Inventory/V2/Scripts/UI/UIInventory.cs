using Godot;
using GodotUtils;
using System.Collections.Generic;

namespace Template.InventoryV2;

public class UIInventory
{
    private List<ItemContainer> _itemContainers = [];
    private CursorManager _cursorManager;

    public UIInventory(Inventory inventory, Node parent)
    {
        // Create the InventoryContainer
        InventoryContainer invContainer = InventoryContainer.Instantiate();

        // Add InventoryContainer to the tree as soon as possible so _Ready() functions in
        // children nodes will get called at the correct times
        parent.AddChild(invContainer);

        // Add the item containers
        AddItemContainers(invContainer, inventory);

        inventory.OnItemChanged += (index, item) =>
        {
            _itemContainers[index].SetItem(item);
        };

        _cursorManager = new CursorManager(parent.GetSceneNode<CursorItemContainer>());
    }

    private void AddItemContainers(InventoryContainer invContainer, Inventory inv)
    {
        for (int i = 0; i < inv.GetInventorySize(); i++)
        {
            AddItemContainer(invContainer, inv, i);
        }
    }

    private void AddItemContainer(InventoryContainer invContainer, Inventory inv, int index)
    {
        ItemContainer itemContainer = invContainer.AddItemContainer();
        _itemContainers.Add(itemContainer);

        itemContainer.SetItem(inv.GetItem(index));

        itemContainer.MouseEntered += () =>
        {

        };

        itemContainer.MouseExited += () =>
        {

        };

        itemContainer.GuiInput += inputEvent =>
        {
            if (inputEvent is InputEventMouseButton mouseBtn)
            {
                if (mouseBtn.IsLeftClickPressed())
                {
                    HandleLeftClick(inv, itemContainer, index);
                }
            }
        };
    }

    private void HandleLeftClick(Inventory inv, ItemContainer itemContainer, int index)
    {
        if (_cursorManager.HasItem())
        {
            if (inv.HasItem(index))
            {
                
            }
            else
            {
                HandlePlaceItem(inv, itemContainer, index);
            }
        }
        else
        {
            if (inv.HasItem(index))
            {
                HandlePickupItem(inv, itemContainer, index);
            }
        }
    }

    private void HandlePlaceItem(Inventory inv, ItemContainer itemContainer, int index)
    {
        Item cursorItem = _cursorManager.GetItem();
        _cursorManager.ClearItem();

        inv.SetItem(index, cursorItem);
    }

    private void HandlePickupItem(Inventory inv, ItemContainer itemContainer, int index)
    {
        Item item = inv.GetItem(index);
        inv.ClearItem(index);

        _cursorManager.SetItem(item, itemContainer.GlobalPosition);
    }
}
