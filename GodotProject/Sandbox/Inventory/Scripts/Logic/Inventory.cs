using System;

namespace Template.Inventory;

public class Inventory : BaseInventory
{
    public event Action<int, ItemStack> OnItemChanged;

    public Inventory(int size) : base(size)
    {
        base.OnItemChanged += (index, item) => OnItemChanged?.Invoke(index, item);
    }

    public void MoveItemTo(BaseInventory other, int fromIndex, int toIndex) => base.MoveItemTo(other, fromIndex, toIndex);
    public void TakeItemFrom(BaseInventory other, int fromIndex, int toIndex) => base.TakeItemFrom(other, fromIndex, toIndex);
    public void SetItem(int index, ItemStack item) => base.SetItem(index, item);
    public void AddItem(ItemStack item) => base.AddItem(item);
    public void RemoveItem(int index) => base.RemoveItem(index);
    public void SwapItems(int index1, int index2) => base.SwapItems(index1, index2);
    public bool HasItem(int index) => GetItem(index) != null;
    public ItemStack GetItem(int index) => base.GetItem(index);
    public int GetInventorySize() => base.GetInventorySize();
}
