using System;
using System.Text;
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

    protected void SetItem(int index, Item item)
    {
        ValidateIndex(index);

        Item newItem = new(item);

        _items[index] = newItem;

        NotifyItemChanged(index, item);
    }

    protected void AddItem(Item item, int count)
    {
        if (FindFirstEmptySlot(out int index))
        {
            Item newItem = new(item);
            newItem.Count = count;

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

    private void NotifyItemChanged(int index, Item item)
    {
        OnItemChanged?.Invoke(index, item);
    }

    public override string ToString()
    {
        StringBuilder strBuilder = new();

        strBuilder.AppendLine("Inventory");

        for (int i = 0; i < _items.Length; i++)
        {
            strBuilder.AppendLine($"Slot {i}: {(_items[i] != null ? _items[i].ToString() : "Empty")}");
        }

        return strBuilder.ToString();
    }
}
