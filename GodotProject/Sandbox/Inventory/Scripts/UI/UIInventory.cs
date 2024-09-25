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

        for (int i = 0; i < inv.GetInventorySize(); i++)
        {
            AddItemContainer(itemContainers, new InventorySlotContext(cursorManager, inv, invContainer.AddItemContainer(), i));
        }

        UpdateItemContainerOnInvChanged(itemContainers, inv);
    }

    private void UpdateItemContainerOnInvChanged(List<ItemContainer> itemContainers, Inventory inv)
    {
        inv.OnItemChanged += (index, item) =>
        {
            itemContainers[index].SetItem(item);
        };
    }

    private void AddItemContainer(List<ItemContainer> itemContainers, InventorySlotContext context)
    {
        ItemContainer itemContainer = context.ItemContainer;
        itemContainer.SetItem(context.Inventory.GetItem(context.Index));

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
                    HandleLeftClick(context);
                }
            }
        };

        itemContainers.Add(itemContainer);
    }

    private void HandleLeftClick(InventorySlotContext context)
    {
        Inventory inv = context.Inventory;
        int index = context.Index;

        if (context.CursorManager.HasItem())
        {
            if (inv.HasItem(index))
            {
                HandleSwapItems(context);
            }
            else
            {
                HandlePlaceItem(context);
            }
        }
        else
        {
            if (inv.HasItem(index))
            {
                HandlePickupItem(context);
            }
        }
    }

    private void HandleSwapItems(InventorySlotContext context)
    {
        CursorManager cursorManager = context.CursorManager;

        // Get the item from the cursor
        Item cursorItem = cursorManager.GetItem();
        int cursorSpriteFrame = cursorManager.GetCurrentSpriteFrame();

        // Get the item from the inventory
        Inventory inv = context.Inventory;
        Item invItem = inv.GetItem(context.Index);
        int invSpriteFrame = context.ItemContainer.GetCurrentSpriteFrame();

        // Set the inv item with the cursor item
        inv.SetItem(context.Index, cursorItem);
        context.ItemContainer.SetCurrentSpriteFrame(cursorSpriteFrame);

        // Set the cursor item with the inv item
        cursorManager.SetItem(invItem, context.ItemContainer.GlobalPosition, invSpriteFrame);
    }

    private void HandlePlaceItem(InventorySlotContext context)
    {
        CursorManager cursorManager = context.CursorManager;

        // Get the item and sprite frame before clearing the item from the cursor
        Item cursorItem = cursorManager.GetItem();
        int spriteFrame = cursorManager.GetCurrentSpriteFrame();

        // Clear the item from the cursor
        cursorManager.ClearItem();

        // Set the inventory item
        context.Inventory.SetItem(context.Index, cursorItem);
        context.ItemContainer.SetCurrentSpriteFrame(spriteFrame);
    }

    private void HandlePickupItem(InventorySlotContext context)
    {
        Inventory inv = context.Inventory;
        
        // Get the item and sprite frame before clearing the item from the inventory
        Item item = inv.GetItem(context.Index);
        int spriteFrame = context.ItemContainer.GetCurrentSpriteFrame();

        // Clear the item from the inventory
        inv.ClearItem(context.Index);

        // Set the cursor item
        context.CursorManager.SetItem(item, context.ItemContainer.GlobalPosition, spriteFrame);
    }
}
