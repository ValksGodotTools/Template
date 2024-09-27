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

        // Add the item containers
        AddItemContainers(invContainer, inventory);
    }

    private void AddItemContainers(InventoryContainer invContainer, Inventory inv)
    {
        int invSize = inv.GetInventorySize();
        ItemContainer[] itemContainers = new ItemContainer[invSize];

        for (int i = 0; i < invSize; i++)
        {
            CursorItemContainer cursorItemContainer = Services.Get<CursorItemContainer>();
            InventorySlotContext context = new(cursorItemContainer, inv, invContainer.AddItemContainer(), i);
            
            AddItemContainer(i, itemContainers, context);
        }

        inv.OnItemChanged += (index, item) =>
        {
            itemContainers[index].SetItem(item);
        };
    }

    private void AddItemContainer(int index, ItemContainer[] itemContainers, InventorySlotContext context)
    {
        ItemContainer itemContainer = context.ItemContainer;
        itemContainer.SetItem(context.Inventory.GetItem(context.Index));

        Inventory inv = context.Inventory;
        CursorInventory cInv = context.CursorInventory;
        InventoryManager invM = context.InventoryManager;
        CursorManager cM = context.CursorManager;

        itemContainer.MouseEntered += () =>
        {
            InputDetector input = Services.Get<InputDetector>();

            if (input.HoldingRightClick)
            {
                if (cInv.HasItem())
                {
                    if (inv.HasItem(index))
                    {
                        if (cInv.GetItem().Equals(inv.GetItem(index)))
                        {
                            // Stack one of the cursor item onto the inventory item

                            // Add one to item
                            invM.GetItemAndFrame(out Item item, out int frame);
                            item.AddCount(1);
                            invM.SetItemAndFrame(item, frame);

                            // Remove one from cursor item
                            Item cItem = cInv.GetItem();
                            cItem.RemoveCount(1);

                            // Update cursor
                            if (cItem.Count <= 0)
                            {
                                cInv.ClearItem();
                            }
                            else
                            {
                                cM.GetItemAndFrame(out Item _, out int cFrame);
                                cM.SetItemAndFrame(cItem, cFrame);
                            }
                        }
                        else
                        {

                        }
                    }
                    else
                    {

                    }
                }
            }
        };

        itemContainer.GuiInput += inputEvent =>
        {
            if (inputEvent is InputEventMouseButton mouseBtn)
            {
                if (mouseBtn.IsLeftClickJustPressed())
                {
                    // Cursor has item
                    if (cInv.HasItem())
                    {
                        // Cursor and inventory have items
                        if (inv.HasItem(index))
                        {
                            if (cInv.GetItem().Equals(inv.GetItem(index)))
                            {
                                // Stack the cursor item onto the inventory item
                                cM.GetItemAndFrame(out Item cItem, out int cFrame);

                                cItem.AddCount(inv.GetItem(index).Count);

                                invM.SetItemAndFrame(cItem, cFrame);

                                cInv.ClearItem();
                            }
                            else
                            {
                                // Swap cursor item with inventory item
                                invM.GetItemAndFrame(out Item item, out int frame);
                                cM.GetItemAndFrame(out Item cItem, out int cFrame);

                                invM.SetItemAndFrame(cItem, cFrame);
                                cM.SetItemAndFrame(item, frame);
                                cM.SetPosition(itemContainer.GlobalPosition);
                            }
                        }
                        // Cursor has item but inventory does not
                        else
                        {
                            // Place cursor item
                            cM.GetItemAndFrame(out Item cItem, out int cFrame);
                            invM.SetItemAndFrame(cItem, cFrame);

                            cInv.ClearItem();
                        }
                    }
                    // Cursor has no item
                    else
                    {
                        // Cursor has no item but inventory item exists
                        if (inv.HasItem(index))
                        {
                            // Pickup inventory item
                            invM.GetItemAndFrame(out Item item, out int frame);

                            cM.SetItemAndFrame(item, frame);
                            cM.SetPosition(itemContainer.GlobalPosition);

                            inv.ClearItem(index);
                        }
                    }
                }
                else if (mouseBtn.IsRightClickJustPressed())
                {
                    // Cursor has item
                    if (cInv.HasItem())
                    {
                        // Cursor and inventory have items
                        if (inv.HasItem(index))
                        {
                            if (cInv.GetItem().Equals(inv.GetItem(index)))
                            {
                                // Stack one of the cursor item onto the inventory item

                                // Add one to item
                                invM.GetItemAndFrame(out Item item, out int frame);
                                item.AddCount(1);
                                invM.SetItemAndFrame(item, frame);

                                // Remove one from cursor item
                                Item cItem = cInv.GetItem();
                                cItem.RemoveCount(1);

                                // Update cursor
                                if (cItem.Count <= 0)
                                {
                                    cInv.ClearItem();
                                }
                                else
                                {
                                    cM.GetItemAndFrame(out Item _, out int cFrame);
                                    cM.SetItemAndFrame(cItem, cFrame);
                                }
                            }
                            else
                            {
                                // Swap cursor item with inventory item
                                invM.GetItemAndFrame(out Item item, out int frame);
                                cM.GetItemAndFrame(out Item cItem, out int cFrame);

                                invM.SetItemAndFrame(cItem, cFrame);
                                cM.SetItemAndFrame(item, frame);
                                cM.SetPosition(itemContainer.GlobalPosition);
                            }
                        }
                        // Cursor has item but inventory does not
                        else
                        {
                            // Place one of cursor item
                            cM.GetItemAndFrame(out Item cItem, out int cFrame);
                            
                            Item nItem = new(cItem);
                            nItem.SetCount(1);

                            invM.SetItemAndFrame(nItem, cFrame);

                            // Update cursor
                            cItem.RemoveCount(1);

                            if (cItem.Count <= 0)
                            {
                                cInv.ClearItem();
                            }
                            else
                            {
                                //cM.GetItemAndFrame(out Item _, out int _);
                                cM.SetItemAndFrame(cItem, cFrame);
                            }
                        }
                    }
                    // Cursor has no item
                    else
                    {
                        // Cursor has no item but inventory item exists
                        if (inv.HasItem(index))
                        {
                            // Pickup half of the inventory item
                            invM.GetItemAndFrame(out Item item, out int frame);

                            Item nItem = new(item);

                            int halfCount = item.Count / 2;

                            // Prevent halfCount from equaling zero
                            if (item.Count == 1)
                            {
                                halfCount = 1;
                            }

                            nItem.SetCount(halfCount);

                            item.RemoveCount(halfCount);

                            if (item.Count <= 0)
                            {
                                inv.ClearItem(index);
                            }
                            else
                            {
                                invM.SetItemAndFrame(item, frame);
                            }

                            cM.SetItemAndFrame(nItem, frame);
                            cM.SetPosition(itemContainer.GlobalPosition);
                        }
                    }
                }
            }
        };

        itemContainers[index] = itemContainer;
    }
}
