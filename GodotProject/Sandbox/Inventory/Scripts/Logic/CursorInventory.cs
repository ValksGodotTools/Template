using System;

namespace Template.Inventory;

// Wrapper class for Inventory. CursorInventory's should only be able to hold one type of item.
public class CursorInventory
{
    public event Action<Item> OnItemChanged;

    // Please excuse the naming convention used here, this is to indicate to developers
    // this should not be directly accessed
    public readonly Inventory _inventory;

    public CursorInventory()
    {
        _inventory = new Inventory(1);
        _inventory.OnItemChanged += (index, item) => OnItemChanged?.Invoke(item);
    }

    public void SetItem(Item item) => _inventory.SetItem(0, item);
    public void RemoveItem() => _inventory.RemoveItem(0);
    public void DebugPrintInventory() => _inventory.DebugPrintInventory();
    public bool HasItem() => _inventory.HasItem(0);
    public Item GetItem() => _inventory.GetItem(0);
}
