namespace Template.InventoryV1;

public class Item
{
    public string Name { get; }
    public int Count { get; set; }
    public ItemTexture Texture { get; set; }

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
        Texture = other.Texture;
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
