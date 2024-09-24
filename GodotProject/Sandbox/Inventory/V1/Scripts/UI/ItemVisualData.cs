using Godot;

namespace Template.InventoryV1;

public class ItemVisualData
{
    public Resource Resource { get; }
    public Color Color { get; }

    public ItemVisualData(Resource resource, Color color)
    {
        Resource = resource;
        Color = color;
    }

    public ItemVisualData(Resource resource)
    {
        Resource = resource;
        Color = Colors.White;
    }
}
