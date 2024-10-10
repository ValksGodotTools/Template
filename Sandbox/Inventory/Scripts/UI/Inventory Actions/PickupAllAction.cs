using Godot;
using System.Collections.Generic;

namespace Template.Inventory;

public class PickupAllAction : IInventoryAction
{
    public void Execute(InventoryContext context, MouseButton mouseBtn, int index)
    {
        if (mouseBtn == MouseButton.Left)
        {
            InventoryContainer container = context.InventoryContainer;
            Inventory cursorInventory = context.CursorInventory;
            Inventory inventory = context.Inventory;

            Material? material = cursorInventory.GetItem(0)?.Material ?? inventory.GetItem(index)?.Material;

            if (material == null)
                return;

            InventoryContainer otherInvContainer = Services.Get<InventorySandbox>().GetOtherInventory(container);

            Dictionary<InventoryContainer, List<(int, ItemStack)>> items = [];
            items[container] = [];
            items[otherInvContainer] = [];

            foreach ((int i, ItemStack item) in inventory.GetItems())
            {
                if (item.Material.Equals(material))
                {
                    items[container].Add((i, item));
                }
            }

            foreach ((int i, ItemStack item) in otherInvContainer.Inventory.GetItems())
            {
                if (item.Material.Equals(material))
                {
                    items[otherInvContainer].Add((i, item));
                }
            }

            int sameItemCount = items[container].Count + items[otherInvContainer].Count;

            if (sameItemCount == 0)
                return;

            foreach ((int i, ItemStack item) in items[container])
            {
                // Do not animate index under cursor
                if (i == index)
                    continue;

                //OnPrePickup?.Invoke(container, i);
                cursorInventory.TakeItemFrom(inventory, i, 0);
                //OnPostPickup?.Invoke(container, i);
            }

            foreach ((int i, ItemStack item) in items[otherInvContainer])
            {
                //OnPrePickup?.Invoke(otherInvContainer, i);
                cursorInventory.TakeItemFrom(otherInvContainer.Inventory, i, 0);
                //OnPostPickup?.Invoke(otherInvContainer, i);
            }
        }
    }
}
