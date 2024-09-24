﻿using System;
using Godot;

namespace Template.InventoryV2;

public class Inventory
{
    private Item[] _items;

    public Inventory(int size)
    {
        if (size <= 0)
        {
            throw new ArgumentException("Inventory size must be greater than zero.");
        }

        _items = new Item[size];
    }

    public void SetItem(int index, Item item, int count = 1)
    {
        ValidateIndex(index);
        _items[index] = item;
        item.Count = count;
    }

    public void AddItem(Item item, int count = 1)
    {
        int index = FindFirstEmptySlot();

        if (index != -1)
        {
            _items[index] = item;
            item.Count = count;
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
        }
        else
        {
            _items[index].Count -= count;
        }
    }

    public void SwapItems(int index1, int index2)
    {
        ValidateIndex(index1);
        ValidateIndex(index2);

        (_items[index2], _items[index1]) = (_items[index1], _items[index2]);
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

    private int FindFirstEmptySlot()
    {
        for (int i = 0; i < _items.Length; i++)
        {
            if (_items[i] == null)
            {
                return i;
            }
        }
        return -1;
    }
}
