using System;

namespace Template.Inventory;

public class CursorInventory : BaseInventory
{
    public event Action<ItemStack> OnItemChanged;

    public CursorInventory() : base(1)
    {
        base.OnItemChanged += (index, item) => OnItemChanged?.Invoke(item);
    }

    public void MoveItemTo(BaseInventory other, int toIndex) => base.MoveItemTo(other, 0, toIndex);
    public void TakeItemFrom(BaseInventory other, int fromIndex) => base.TakeItemFrom(other, fromIndex, 0);
    public void SetItem(ItemStack item) => base.SetItem(0, item);
    public void RemoveItem() => base.RemoveItem(0);
    public bool HasItem() => GetItem() != null;
    public ItemStack GetItem() => base.GetItem(0);

    public override string ToString()
    {
        return GetItem().ToString();
    }
}
