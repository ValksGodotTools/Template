using System;
using Godot;

namespace Template.Inventory;

public class Inventory
{
    public event Action<Item, int> OnItemChanged;

    private Item[] _items;

    public Inventory(int size)
    {
        if (size <= 0)
        {
            throw new ArgumentException("Inventory size must be greater than zero.");
        }

        _items = new Item[size];
    }

    public void SetItem(Item item, int index = 0)
    {
        ValidateIndex(index);

        Item newItem = new(item);

        _items[index] = newItem;

        OnItemChanged?.Invoke(item, index);
    }

    public void AddItem(Item item, int count = 1)
    {
        if (FindFirstEmptySlot(out int index))
        {
            Item newItem = new(item);
            newItem.SetCount(count);

            _items[index] = newItem;

            OnItemChanged?.Invoke(newItem, index);
        }
        else
        {
            GD.Print("Inventory is full.");
        }
    }

    public void RemoveItem(int index = 0)
    {
        ValidateIndex(index);

        _items[index] = null;

        OnItemChanged?.Invoke(null, index);
    }

    public void SwapItems(int index1, int index2)
    {
        ValidateIndex(index1);
        ValidateIndex(index2);

        (_items[index2], _items[index1]) = (_items[index1], _items[index2]);

        OnItemChanged?.Invoke(_items[index1], index1);
        OnItemChanged?.Invoke(_items[index2], index2);
    }

    public bool HasItem(int index = 0)
    {
        return GetItem(index) != null;
    }

    public Item GetItem(int index = 0)
    {
        ValidateIndex(index);

        return _items[index];
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
