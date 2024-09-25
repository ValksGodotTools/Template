using System;
using Godot;

namespace Template.Inventory;

public class Inventory
{
    public event Action<int, Item> OnItemChanged;

    private Item[] _items;

    public Inventory(int size)
    {
        if (size <= 0)
        {
            throw new ArgumentException("Inventory size must be greater than zero.");
        }

        _items = new Item[size];
    }

    public void SetItem(int index, Item item)
    {
        ValidateIndex(index);

        Item newItem = new(item);

        _items[index] = newItem;

        OnItemChanged?.Invoke(index, newItem);
    }

    public void AddItem(Item item, int count = 1)
    {
        if (FindFirstEmptySlot(out int index))
        {
            Item newItem = new(item)
            {
                Count = count
            };

            _items[index] = newItem;

            OnItemChanged?.Invoke(index, newItem);
        }
        else
        {
            GD.Print("Inventory is full.");
        }
    }

    public void ClearItem(int index)
    {
        ValidateIndex(index);

        _items[index] = null;

        OnItemChanged?.Invoke(index, null);
    }

    public void RemoveItem(int index, int count = 1)
    {
        ValidateIndex(index);

        if (_items[index] == null)
        {
            GD.Print("No item at the specified index.");
            return;
        }

        if (_items[index].Count <= count)
        {
            _items[index] = null;

            OnItemChanged?.Invoke(index, null);
        }
        else
        {
            _items[index].Count -= count;

            OnItemChanged?.Invoke(index, _items[index]);
        }
    }

    public void SwapItems(int index1, int index2)
    {
        ValidateIndex(index1);
        ValidateIndex(index2);

        (_items[index2], _items[index1]) = (_items[index1], _items[index2]);

        OnItemChanged?.Invoke(index1, _items[index1]);
        OnItemChanged?.Invoke(index2, _items[index2]);
    }

    public bool HasItem(int index)
    {
        return GetItem(index) != null;
    }

    public Item GetItem(int index)
    {
        ValidateIndex(index);

        return _items[index];
    }

    public int GetItemCount()
    {
        int count = 0;

        foreach (Item item in _items)
        {
            if (item != null)
            {
                count++;
            }
        }
        return count;
    }

    public int GetInventorySize()
    {
        return _items.Length;
    }

    public void DebugPrintInventory()
    {
        GD.Print("Inventory");

        for (int i = 0; i < _items.Length; i++)
        {
            GD.Print($"Slot {i}: {(_items[i] != null ? _items[i].ToString() : "Empty")}");
        }
    }

    private void ValidateIndex(int index)
    {
        if (index < 0 || index >= _items.Length)
        {
            throw new IndexOutOfRangeException("Index out of range.");
        }
    }

    private bool FindFirstEmptySlot(out int index)
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
}
