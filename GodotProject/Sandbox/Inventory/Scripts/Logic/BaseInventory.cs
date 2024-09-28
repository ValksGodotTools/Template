using System;
using System.Collections.Generic;
using System.Linq;
using Godot;

namespace Template.Inventory;

public class BaseInventory
{
    protected event Action<int, ItemStack> OnItemChanged;
    
    private ItemStack[] _itemStacks;

    protected BaseInventory(int size)
    {
        if (size <= 0)
        {
            throw new ArgumentException("Inventory size must be greater than zero.");
        }

        _itemStacks = new ItemStack[size];
    }

    public void Clear()
    {
        for (int i = 0; i < _itemStacks.Length; i++)
        {
            RemoveItem(i);
        }
    }

    public IEnumerable<ItemStack> GetItems() => _itemStacks.Where(item => item != null);

    protected void MoveItemTo(BaseInventory other, int fromIndex, int toIndex)
    {
        ItemStack item = GetItem(fromIndex);

        other.SetItem(toIndex, item);

        RemoveItem(fromIndex);
    }

    protected void TakeItemFrom(BaseInventory other, int fromIndex, int toIndex)
    {
        ItemStack item = other.GetItem(fromIndex);

        SetItem(toIndex, item);

        other.RemoveItem(fromIndex);
    }

    protected void SetItem(int index, ItemStack item)
    {
        ValidateIndex(index);

        _itemStacks[index] = item;

        NotifyItemChanged(index, item);
    }

    protected void AddItem(ItemStack item)
    {
        // Try to stack the item with an existing item in the inventory
        if (TryStackItem(item))
        {
            return;
        }

        // If the item cannot be stacked, try to place the item in the first empty slot
        if (TryFindFirstEmptySlot(out int index))
        {
            _itemStacks[index] = item;
            NotifyItemChanged(index, item);
        }
        else
        {
            GD.Print("Inventory is full.");
        }
    }


    protected void RemoveItem(int index)
    {
        ValidateIndex(index);

        _itemStacks[index] = null;

        NotifyItemChanged(index, null);
    }

    protected void SwapItems(int index1, int index2)
    {
        ValidateIndex(index1);
        ValidateIndex(index2);

        (_itemStacks[index2], _itemStacks[index1]) = (_itemStacks[index1], _itemStacks[index2]);

        NotifyItemChanged(index1, _itemStacks[index1]);
        NotifyItemChanged(index2, _itemStacks[index2]);
    }

    protected bool HasItem(int index)
    {
        return GetItem(index) != null;
    }

    protected ItemStack GetItem(int index)
    {
        ValidateIndex(index);

        return _itemStacks[index];
    }

    protected int GetInventorySize()
    {
        return _itemStacks.Length;
    }

    private void ValidateIndex(int index)
    {
        if (index < 0 || index >= _itemStacks.Length)
        {
            throw new IndexOutOfRangeException("Index out of range.");
        }
    }

    private bool TryStackItem(ItemStack item)
    {
        for (int i = 0; i < _itemStacks.Length; i++)
        {
            if (_itemStacks[i] != null && _itemStacks[i].Equals(item))
            {
                _itemStacks[i].Add(item.Count);
                NotifyItemChanged(i, _itemStacks[i]);
                return true;
            }
        }

        return false;
    }

    private bool TryFindFirstEmptySlot(out int index)
    {
        for (int i = 0; i < _itemStacks.Length; i++)
        {
            if (_itemStacks[i] == null)
            {
                index = i;
                return true;
            }
        }

        index = -1;

        return false;
    }

    private void NotifyItemChanged(int index, ItemStack item)
    {
        OnItemChanged?.Invoke(index, item);
    }

    public void DebugPrintInventory()
    {
        GD.Print(GetType().Name);

        for (int i = 0; i < _itemStacks.Length; i++)
        {
            GD.Print($"Slot {i}: {(_itemStacks[i] != null ? _itemStacks[i].ToString() : "Empty")}");
        }
    }

    public override string ToString()
    {
        return string.Join(' ', _itemStacks.Where(item => item != null));
    }
}
