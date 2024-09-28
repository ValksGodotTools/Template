using System;

namespace Template.Inventory;

public class Inventory : BaseInventory
{
    public event Action<int, Item> OnItemChanged;

    public Inventory(int size) : base(size)
    {
        base.OnItemChanged += (index, item) => OnItemChanged?.Invoke(index, item);
    }

    public void SetItem(int index, Item item) => base.SetItem(index, item);
    public void AddItem(Item item, int count) => base.AddItem(item, count);
    public void RemoveItem(int index) => base.RemoveItem(index);
    public void SwapItems(int index1, int index2) => base.SwapItems(index1, index2);
    public bool HasItem(int index) => GetItem(index) != null;
    public Item GetItem(int index) => base.GetItem(index);
    public int GetInventorySize() => base.GetInventorySize();
}
