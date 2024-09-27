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

    public virtual void SetItem(int index, Item item)
    {
        ValidateIndex(index);

        Item newItem = new(item);

        _items[index] = newItem;

        OnItemChanged?.Invoke(index, item);
    }

    public virtual void AddItem(Item item, int count = 1)
    {
        if (FindFirstEmptySlot(out int index))
        {
            Item newItem = new(item);
            newItem.SetCount(count);

            _items[index] = newItem;

            OnItemChanged?.Invoke(index, newItem);
        }
        else
        {
            GD.Print("Inventory is full.");
        }
    }

    public virtual void RemoveItem(int index)
    {
        ValidateIndex(index);

        _items[index] = null;

        OnItemChanged?.Invoke(index, null);
    }

    public virtual void SwapItems(int index1, int index2)
    {
        ValidateIndex(index1);
        ValidateIndex(index2);

        (_items[index2], _items[index1]) = (_items[index1], _items[index2]);

        OnItemChanged?.Invoke(index1, _items[index1]);
        OnItemChanged?.Invoke(index2, _items[index2]);
    }

    public virtual bool HasItem(int index)
    {
        return GetItem(index) != null;
    }

    public virtual Item GetItem(int index)
    {
        ValidateIndex(index);

        return _items[index];
    }

    public virtual int GetInventorySize()
    {
        return _items.Length;
    }

    public virtual void DebugPrintInventory()
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
