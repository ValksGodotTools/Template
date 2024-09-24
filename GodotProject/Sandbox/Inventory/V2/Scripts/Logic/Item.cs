using Godot;

namespace Template.InventoryV2;

public class Item
{
    public string Name { get; private set; }
    public int Count { get; set; }
    public Color Color { get; private set; }
    public string ResourcePath { get; private set; }

    private Item(string name, int count = 1)
    {
        Name = name;
        Count = count;
    }

    public Item Clone(Item other)
    {
        return new(other.Name, other.Count)
        {
            ResourcePath = other.ResourcePath,
            Color = other.Color
        };
    }

    public bool Equals(Item other)
    {
        if (other == null)
            return false;

        return Name == other.Name;
    }

    public override string ToString()
    {
        return $"{Name} (x{Count})";
    }

    public class Builder(string name, int count = 1)
    {
        private const string BaseResourcePath = "res://Sandbox/Inventory/";

        private Item _item = new(name, count);

        public Builder SetResourcePath(string resourcePath)
        {
            _item.ResourcePath = $"{BaseResourcePath}{resourcePath}";
            return this;
        }

        public Builder SetColor(Color color)
        {
            _item.Color = color;
            return this;
        }

        public Item Build() => _item;
    }
}
