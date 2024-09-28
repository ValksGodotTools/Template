using System;
using System.Collections.Generic;
using System.Linq;
using Godot;

namespace Template.Inventory;

public class BaseInventory
{
    protected event Action<int, Item> OnItemChanged;
    
    private Item[] _items;

    protected BaseInventory(int size)
    {
        if (size <= 0)
        {
            throw new ArgumentException("Inventory size must be greater than zero.");
        }

        _items = new Item[size];
    }

    public void Clear()
    {
        for (int i = 0; i < _items.Length; i++)
        {
            RemoveItem(i);
        }
    }

    public IEnumerable<Item> GetItems() => _items.Where(item => item != null);

    protected void SetItem(int index, Item item)
    {
        ValidateIndex(index);

        Item newItem = new(item);

        _items[index] = newItem;

        NotifyItemChanged(index, item);
    }

    protected void AddItem(Item item, int count)
    {
        item.Count = count;

        // Try to stack the item with an existing item in the inventory
        if (TryStackItem(item))
        {
            return;
        }

        // If the item cannot be stacked, try to place the item in the first empty slot
        if (TryFindFirstEmptySlot(out int index))
        {
            Item newItem = new(item);
            _items[index] = newItem;
            NotifyItemChanged(index, newItem);
        }
        else
        {
            GD.Print("Inventory is full.");
        }
    }


    protected void RemoveItem(int index)
    {
        ValidateIndex(index);

        _items[index] = null;

        NotifyItemChanged(index, null);
    }

    protected void SwapItems(int index1, int index2)
    {
        ValidateIndex(index1);
        ValidateIndex(index2);

        (_items[index2], _items[index1]) = (_items[index1], _items[index2]);

        NotifyItemChanged(index1, _items[index1]);
        NotifyItemChanged(index2, _items[index2]);
    }

    protected bool HasItem(int index)
    {
        return GetItem(index) != null;
    }

    protected Item GetItem(int index)
    {
        ValidateIndex(index);

        return _items[index];
    }

    protected int GetInventorySize()
    {
        return _items.Length;
    }

    private void ValidateIndex(int index)
    {
        if (index < 0 || index >= _items.Length)
        {
            throw new IndexOutOfRangeException("Index out of range.");
        }
    }

    private bool TryStackItem(Item item)
    {
        for (int i = 0; i < _items.Length; i++)
        {
            if (_items[i] != null && _items[i].Equals(item))
            {
                _items[i].Count += item.Count;
                NotifyItemChanged(i, _items[i]);
                return true;
            }
        }

        return false;
    }

    private bool TryFindFirstEmptySlot(out int index)
    {
        for (int i = 0; i < _items.Length; i++)
        {
            if (_items[i] == null)
            {
                index = i;
                return true;
            }
        }

        index = -1;

        return false;
    }

    private void NotifyItemChanged(int index, Item item)
    {
        OnItemChanged?.Invoke(index, item);
    }

    public void DebugPrintInventory()
    {
        GD.Print(GetType().Name);

        for (int i = 0; i < _items.Length; i++)
        {
            GD.Print($"Slot {i}: {(_items[i] != null ? _items[i].ToString() : "Empty")}");
        }
    }

    public override string ToString()
    {
        return string.Join(' ', _items.Where(item => item != null));
    }
}
