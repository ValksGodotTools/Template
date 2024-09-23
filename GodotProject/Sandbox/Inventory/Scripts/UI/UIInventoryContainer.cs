using Godot;
using GodotUtils;

namespace Template.Inventory;

public class UIInventoryContainer : UIContainerBase
{
    private const int SEPARATION = 5;
    private const int ITEM_CONTAINER_SIZE = 50;

    private readonly PanelContainer _container;
    private UIInventoryItemContainer[] _itemContainers;

    public UIInventoryContainer(Inventory inventory, int columns = 10)
    {
        _container = new PanelContainer();
        GridContainer grid = AddGridContainer(columns);

        _itemContainers = new UIInventoryItemContainer[inventory.GetItemCount()];

        for (int i = 0; i < inventory.GetItemCount(); i++)
        {
            UIInventoryItemContainer container = new(ITEM_CONTAINER_SIZE);
            _itemContainers[i] = container;

            ItemVisualData itemVisualData = ItemSpriteManager.GetResource(inventory.GetItem(i));

            UIInventoryItemSprite sprite = ResourceFactoryRegistry.CreateSprite(itemVisualData);

            SetItem(i, sprite);

            grid.AddChild(container.Build());
        }
    }

    public void SetItem(int index, UIInventoryItemSprite sprite)
    {
        _itemContainers[index].SetItemSprite(sprite);
    }

    public override PanelContainer Build()
    {
        return _container;
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
}
