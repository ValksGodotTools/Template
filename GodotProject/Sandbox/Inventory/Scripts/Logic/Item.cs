using System;

namespace Template.Inventory;

public class Item
{
    public event Action<int> OnCountChanged;

    public string Name { get; private set; }
    public string Description { get; private set; }
    public int Count { get; private set; }

    private Item(string name, int count = 1)
    {
        Name = name;
        Count = count;
    }

    public Item(Item other)
    {
        Name = other.Name;
        Count = other.Count;
    }

    public void SetCount(int count)
    {
        Count = count;
        OnCountChanged?.Invoke(Count);
    }

    public void AddCount(int count)
    {
        Count += count;
        OnCountChanged?.Invoke(Count);
    }

    public void RemoveCount(int count)
    {
        Count -= count;
        if (count < 0) Count = 0;
        OnCountChanged?.Invoke(Count);
    }

    public bool Equals(Item other)
    {
        if (other == null)
        {
            return false;
        }

        return Name == other.Name;
    }

    public override bool Equals(object obj)
    {
        if (obj == null || GetType() != obj.GetType())
        {
            return false;
        }

        return Equals((Item)obj);
    }

    public override int GetHashCode()
    {
        return Name.GetHashCode();
    }

    public override string ToString()
    {
        return $"{Name} (x{Count})";
    }

    public class Builder(string name, int count = 1)
    {
        private Item _item = new(name, count);

        public Builder SetDescription(string description)
        {
            _item.Description = description;
            return this;
        }

        public Item Build() => _item;
    }
}
