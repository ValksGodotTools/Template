using Godot;
using GodotUtils;

namespace Template.Inventory;

public class InventoryItemContainer
{
    private readonly Control _itemControlParent;

    public InventoryItemContainer(float size, Node parent)
    {
        PanelContainer container = new()
        {
            CustomMinimumSize = Vector2.One * size
        };

        _itemControlParent = AddCenterItemContainer(container);

        parent.AddChild(container);
    }

    public void SetItemSprite(InventoryItemSprite sprite)
    {
        _itemControlParent.QueueFreeChildren();
        _itemControlParent.AddChild(sprite.Build());
    }

    private Control AddCenterItemContainer(PanelContainer container)
    {
        CenterContainer center = new();
        Control control = new();

        container.AddChild(center);
        center.AddChild(control);

        return control;
    }
}
