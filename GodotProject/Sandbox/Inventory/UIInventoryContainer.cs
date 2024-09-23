using Godot;
using GodotUtils;

namespace Template.Inventory;

public class UIInventoryContainer : UIContainerBase
{
    private const int SEPARATION = 5;
    private const int MARGIN = 20;

    private readonly PanelContainer _container;
    private readonly GridContainer _grid;

    public UIInventoryContainer(int columns)
    {
        _container = new PanelContainer();
        _grid = AddGridContainer(columns);
    }

    public void AddItem(UIInventoryItemSprite sprite)
    {
        UIInventoryItemContainer container = new(sprite.GetSize().X + MARGIN);
        container.AddItemSprite(sprite);
        _grid.AddChild(container.Build());
    }

    private GridContainer AddGridContainer(int columns)
    {
        GMarginContainer margin = new();
        GridContainer grid = new();

        grid.Columns = columns;
        grid.AddThemeConstantOverride("h_separation", SEPARATION);
        grid.AddThemeConstantOverride("v_separation", SEPARATION);

        _container.AddChild(margin);

        margin.AddChild(grid);
        margin.SetMarginAll(SEPARATION);

        return grid;
    }

    public override PanelContainer Build()
    {
        return _container;
    }
}
