using Godot;
using GodotUtils;

namespace Template.Inventory;

public class InventoryItemContainer
{
    private readonly PanelContainer _container;
    private readonly Control _control;

    public InventoryItemContainer(float size, Node parent)
    {
        _container = new PanelContainer
        {
            CustomMinimumSize = Vector2.One * size
        };

        _control = AddCenterItemContainer();

        parent.AddChild(_container);
    }

    public void SetItemSprite(InventoryItemSprite sprite)
    {
        _control.QueueFreeChildren();
        _control.AddChild(sprite.Build());
    }

    private Control AddCenterItemContainer()
    {
        CenterContainer center = new();
        Control control = new();

        _container.AddChild(center);
        center.AddChild(control);

        return control;
    }
}
