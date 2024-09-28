namespace Template.Inventory;

public class Item
{
    public string Name { get; private set; }
    public string Description { get; private set; }

    private Item(string name)
    {
        Name = name;
    }

    public Item(Item other)
    {
        Name = other.Name;
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
        return Name;
    }

    public class Builder(string name)
    {
        private Item _item = new(name);

        public Builder SetDescription(string description)
        {
            _item.Description = description;
            return this;
        }

        public Item Build() => _item;
    }
}
