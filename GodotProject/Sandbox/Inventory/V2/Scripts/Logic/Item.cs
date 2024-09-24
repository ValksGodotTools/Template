using Godot;

namespace Template.InventoryV2;

public class Item
{
    public string Name { get; set; }
    public int Count { get; set; }
    public Color Color { get; set; }
    public string ResourcePath { get; set; }

    public Item(string name, int count = 1)
    {
        Name = name;
        Count = count;
    }

    public Item(Item other, int count = 1) : this(other)
    {
        Count = count;
    }

    public Item(Item other)
    {
        Name = other.Name;
        Count = other.Count;
        ResourcePath = other.ResourcePath;
    }

    public override string ToString()
    {
        return $"{Name} (x{Count})";
    }

    public bool Equals(Item other)
    {
        if (other == null)
            return false;

        return Name == other.Name;
    }
}
