using System.Collections.Generic;
using System;
using Godot;

namespace Template.Inventory;

public class Inventory
{
    private List<Item> _items = [];

    public void SetItem(int index, Item item)
    {
        ValidateIndex(index);
        _items[index] = item;
    }

    public void AddItem(Item item, int count = 1)
    {
        _items.Add(new Item(item) { Count = count });
    }

    public void RemoveItem(Item item)
    {
        _items.Remove(item);
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
        return _items.Count;
    }

    public void PrintInventory()
    {
        GD.Print("Inventory");

        for (int i = 0; i < _items.Count; i++)
        {
            GD.Print($"Slot {i}: {_items[i]}");
        }
    }

    private void ValidateIndex(int index)
    {
        if (index < 0 || index >= _items.Count)
        {
            throw new IndexOutOfRangeException("Index is out of range.");
        }
    }
}
