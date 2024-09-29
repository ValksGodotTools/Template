using System;
using System.Collections.Generic;
using System.Linq;
using Godot;

namespace Template.Inventory;

public class Inventory
{
    public event Action<int, ItemStack> OnItemChanged;
    
    private ItemStack[] _itemStacks;

    public Inventory(int size)
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

    public void MoveItemTo(Inventory other, int fromIndex, int toIndex)
    {
        ItemStack item = GetItem(fromIndex);
        ItemStack otherItem = other.GetItem(toIndex);

        if (item != null && otherItem != null)
        {
            if (item.Material.Equals(otherItem.Material))
            {
                // Stack items
                otherItem.Add(item.Count);
                other.NotifyItemChanged(toIndex, otherItem);

                RemoveItem(fromIndex);
                return;
            }
            else
            {
                other.SetItem(toIndex, item);
                SetItem(fromIndex, otherItem);
                return;
            }
        }
        
        // Place or Pickup items
        other.SetItem(toIndex, item);
        RemoveItem(fromIndex);
    }

    public void TakeItemFrom(Inventory other, int fromIndex, int toIndex)
    {
        ItemStack otherItem = other.GetItem(fromIndex);
        ItemStack item = GetItem(toIndex);

        if (item != null && otherItem != null)
        {
            if (item.Material.Equals(otherItem.Material))
            {
                // Stack items
                item.Add(otherItem.Count);
                NotifyItemChanged(toIndex, item);

                other.RemoveItem(fromIndex);
                return;
            }
            else
            {
                // Swap items
                SetItem(toIndex, otherItem);
                other.SetItem(fromIndex, item);
                return;
            }
        }

        // Place or Pickup items
        SetItem(toIndex, otherItem);
        other.RemoveItem(fromIndex);
    }

    public void SetItem(int index, ItemStack item)
    {
        ValidateIndex(index);

        _itemStacks[index] = item;

        NotifyItemChanged(index, item);
    }

    public void AddItem(ItemStack item)
    {
        // Try to stack the item with an existing item in the inventory
        if (TryStackItemFullSearch(item))
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


    public void RemoveItem(int index)
    {
        ValidateIndex(index);

        _itemStacks[index] = null;

        NotifyItemChanged(index, null);
    }

    public void SwapItems(int index1, int index2)
    {
        ValidateIndex(index1);
        ValidateIndex(index2);

        (_itemStacks[index2], _itemStacks[index1]) = (_itemStacks[index1], _itemStacks[index2]);

        NotifyItemChanged(index1, _itemStacks[index1]);
        NotifyItemChanged(index2, _itemStacks[index2]);
    }

    public bool HasItem(int index)
    {
        return GetItem(index) != null;
    }

    public ItemStack GetItem(int index)
    {
        ValidateIndex(index);

        return _itemStacks[index];
    }

    public int GetInventorySize()
    {
        return _itemStacks.Length;
    }

    private void NotifyItemChanged(int index, ItemStack item)
    {
        OnItemChanged?.Invoke(index, item);
    }

    private void ValidateIndex(int index)
    {
        if (index < 0 || index >= _itemStacks.Length)
        {
            throw new IndexOutOfRangeException("Index out of range.");
        }
    }

    private bool TryStackItemFullSearch(ItemStack item)
    {
        for (int i = 0; i < _itemStacks.Length; i++)
        {
            if (_itemStacks[i] != null && _itemStacks[i].Material.Equals(item.Material))
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
