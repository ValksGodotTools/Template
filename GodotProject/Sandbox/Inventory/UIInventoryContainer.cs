using Godot;
using GodotUtils;
using System;
using System.Collections.Generic;

namespace Template.Inventory;

public class UIInventoryContainer : UIContainerBase
{
    private const int SEPARATION = 5;
    private const int ITEM_CONTAINER_SIZE = 50;

    private readonly PanelContainer _container;
    private UIInventoryItemContainer[] _itemContainers;

    public UIInventoryContainer(int size, int columns = 10)
    {
        _container = new PanelContainer();
        GridContainer grid = AddGridContainer(columns);

        _itemContainers = new UIInventoryItemContainer[size];

        for (int i = 0; i < size; i++)
        {
            UIInventoryItemContainer container = new(ITEM_CONTAINER_SIZE);
            _itemContainers[i] = container;
            grid.AddChild(container.Build());
        }
    }

    public UIInventoryContainer SetItem(int index, UIInventoryItemSprite sprite)
    {
        if (index < 0 || index >= _itemContainers.Length)
        {
            throw new ArgumentOutOfRangeException(nameof(index), "Index is out of range.");
        }

        _itemContainers[index].SetItemSprite(sprite);
        return this;
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
