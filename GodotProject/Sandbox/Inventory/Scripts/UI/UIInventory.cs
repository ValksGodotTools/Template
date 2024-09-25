using Godot;
using GodotUtils;
using System.Collections.Generic;

namespace Template.Inventory;

public class UIInventory
{
    public UIInventory(Inventory inventory, Node parent)
    {
        // Create the InventoryContainer
        InventoryContainer invContainer = InventoryContainer.Instantiate();

        // Add InventoryContainer to the tree as soon as possible so _Ready() functions in
        // children nodes will get called at the correct times
        parent.AddChild(invContainer);

        // Create a cursor manager
        CursorManager cursorManager = new(parent.GetSceneNode<CursorItemContainer>());

        // Add the item containers
        AddItemContainers(cursorManager, invContainer, inventory);
    }

    private void AddItemContainers(CursorManager cursorManager, InventoryContainer invContainer, Inventory inv)
    {
        List<ItemContainer> itemContainers = [];

        List<InventoryInputHandler> inputs = 
        [
            new InventoryInputLeftClick(),
            new InventoryInputRightClick()
        ];

        for (int i = 0; i < inv.GetInventorySize(); i++)
        {
            InventorySlotContext context = new(cursorManager, inv, invContainer.AddItemContainer(), i);
            
            AddItemContainer(inputs, itemContainers, context);
        }

        UpdateItemContainerOnInvChanged(itemContainers, inv);
    }

    private void AddItemContainer(List<InventoryInputHandler> inputs, List<ItemContainer> itemContainers, InventorySlotContext context)
    {
        ItemContainer itemContainer = context.ItemContainer;
        itemContainer.SetItem(context.Inventory.GetItem(context.Index));

        itemContainer.MouseEntered += () =>
        {
            // TODO: Will be used for checking if an item was dropped in the world
        };

        itemContainer.MouseExited += () =>
        {
            // TODO: Will be used for checking if an item was dropped in the world
        };

        itemContainer.GuiInput += inputEvent =>
        {
            if (inputEvent is InputEventMouseButton mouseBtn)
            {
                foreach (InventoryInputHandler input in inputs)
                {
                    if (input.HasInput(mouseBtn))
                    {
                        input.Handle(context);
                    }
                }
            }
        };

        itemContainers.Add(itemContainer);
    }

    private void UpdateItemContainerOnInvChanged(List<ItemContainer> itemContainers, Inventory inv)
    {
        inv.OnItemChanged += (index, item) =>
        {
            itemContainers[index].SetItem(item);
        };
    }
}
