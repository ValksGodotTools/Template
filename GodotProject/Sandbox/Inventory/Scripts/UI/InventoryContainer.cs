using Godot;
using GodotUtils;

namespace Template.Inventory;

public class InventoryContainer
{
    private const int SEPARATION = 5;
    private const int ITEM_CONTAINER_SIZE = 50;

    private readonly PanelContainer _container;
    private readonly GridContainer _grid;
    private InventoryItemContainer[] _itemContainers;

    public InventoryContainer(Inventory inventory, Node parent, int columns = 10)
    {
        _container = new PanelContainer();
        _grid = AddGridContainer(columns);

        _itemContainers = new InventoryItemContainer[inventory.GetItemCount()];

        parent.AddChild(_container);

        AddItems(inventory);
    }

    public void SetItem(int index, Item item)
    {
        ItemVisualData itemVisualData = ItemSpriteManager.GetResource(item);
        InventoryItemSprite sprite = ResourceFactoryRegistry.CreateSprite(itemVisualData);

        _itemContainers[index].SetItemSprite(sprite);

        sprite.SetCount(item.Count);
    }

    private void AddItems(Inventory inventory)
    {
        for (int i = 0; i < inventory.GetItemCount(); i++)
        {
            InventoryItemContainer container = new(ITEM_CONTAINER_SIZE, _grid);
            _itemContainers[i] = container;

            Item item = inventory.GetItem(i);

            SetItem(i, item);
        }
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
