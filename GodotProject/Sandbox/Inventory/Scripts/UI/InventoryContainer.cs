using Godot;
using GodotUtils;

namespace Template.Inventory;

public class InventoryContainer
{
    private InventoryItemContainer[] _itemContainers;

    public InventoryContainer(Inventory inventory, Node parent, int columns = 10)
    {
        _itemContainers = new InventoryItemContainer[inventory.GetItemCount()];

        PanelContainer container = new();
        GridContainer grid = AddGridContainer(container, columns);
        parent.AddChild(container);

        AddItems(inventory, grid);
    }

    public void SetItem(int index, Item item)
    {
        ItemVisualData itemVisualData = ItemSpriteManager.GetResource(item);
        InventoryItemSprite sprite = ResourceFactoryRegistry.CreateSprite(itemVisualData);

        _itemContainers[index].SetItemSprite(sprite);

        sprite.SetCount(item.Count);
    }

    private void AddItems(Inventory inventory, GridContainer grid)
    {
        const int ITEM_CONTAINER_SIZE = 50;

        for (int i = 0; i < inventory.GetItemCount(); i++)
        {
            InventoryItemContainer container = new(ITEM_CONTAINER_SIZE, grid);
            _itemContainers[i] = container;

            Item item = inventory.GetItem(i);

            SetItem(i, item);
        }
    }

    private GridContainer AddGridContainer(PanelContainer container, int columns)
    {
        GMarginContainer margin = new();
        GridContainer grid = new();

        const int SEPARATION = 5;

        grid.Columns = columns;
        grid.AddThemeConstantOverride("h_separation", SEPARATION);
        grid.AddThemeConstantOverride("v_separation", SEPARATION);

        container.AddChild(margin);

        margin.AddChild(grid);
        margin.SetMarginAll(SEPARATION);

        return grid;
    }
}
