using Godot;
using GodotUtils;

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
        int invSize = inv.GetInventorySize();
        ItemContainer[] itemContainers = new ItemContainer[invSize];

        InventoryInputHandler[] inputs = 
        [
            new InventoryInputLeftClick(),
            new InventoryInputRightClick()
        ];

        for (int i = 0; i < invSize; i++)
        {
            InventorySlotContext context = new(cursorManager, inv, invContainer.AddItemContainer(), i);
            
            AddItemContainer(inputs, i, itemContainers, context);
        }

        UpdateItemContainerOnInvChanged(itemContainers, inv);
    }

    private void AddItemContainer(InventoryInputHandler[] inputs, int index, ItemContainer[] itemContainers, InventorySlotContext context)
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

        itemContainers[index] = itemContainer;
    }

    private void UpdateItemContainerOnInvChanged(ItemContainer[] itemContainers, Inventory inv)
    {
        inv.OnItemChanged += (index, item) =>
        {
            itemContainers[index].SetItem(item);
        };
    }
}
