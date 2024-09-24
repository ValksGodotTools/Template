using Godot;
using GodotUtils;

namespace Template.Inventory;

public class InventoryContainer
{
    public Inventory Inventory { get; private set; }

    private InventoryItemContainer[] _itemContainers;
    private MouseEventManager _mouseEventManager;

    public InventoryContainer(Inventory inventory, Node parent, int columns = 10)
    {
        Inventory = inventory;
        InitializeItemContainers(inventory);
        InitializeMouseEventManager();
        CreateAndAddContainerToParent(parent, columns);
    }

    private void InitializeItemContainers(Inventory inventory)
    {
        _itemContainers = new InventoryItemContainer[inventory.GetInventorySize()];
    }

    private void InitializeMouseEventManager()
    {
        _mouseEventManager = new MouseEventManager();
    }

    private void CreateAndAddContainerToParent(Node parent, int columns)
    {
        PanelContainer container = new();
        GridContainer grid = CreateGridContainer(container, columns);
        parent.AddChild(container);
        AddItems(Inventory, grid);
    }

    public void SetItem(int index, Item item)
    {
        _itemContainers[index].SetItem(item);
    }

    private void AddItems(Inventory inventory, GridContainer grid)
    {
        for (int i = 0; i < inventory.GetInventorySize(); i++)
        {
            InventoryItemContainer container = CreateInventoryItemContainer(i, grid);
            _itemContainers[i] = container;
            AttachMouseEvents(container);
            SetItemIfExists(inventory, i, container);
        }
    }

    private InventoryItemContainer CreateInventoryItemContainer(int index, GridContainer grid)
    {
        return new InventoryItemContainer(index, grid, this);
    }

    private void AttachMouseEvents(InventoryItemContainer container)
    {
        container.MouseEntered += _mouseEventManager.OnMouseEntered;
        container.MouseExited += _mouseEventManager.OnMouseExited;
    }

    private void SetItemIfExists(Inventory inventory, int index, InventoryItemContainer container)
    {
        Item item = inventory.GetItem(index);

        if (item != null)
        {
            container.Item = item;
            SetItem(index, item);
        }
    }

    private GridContainer CreateGridContainer(PanelContainer container, int columns)
    {
        GMarginContainer margin = new();
        GridContainer grid = new();

        const int SEPARATION = 5;

        ConfigureGridContainer(grid, columns, SEPARATION);
        AddMarginAndGridToContainer(container, margin, grid, SEPARATION);

        return grid;
    }

    private void ConfigureGridContainer(GridContainer grid, int columns, int separation)
    {
        grid.Columns = columns;
        grid.AddThemeConstantOverride("h_separation", separation);
        grid.AddThemeConstantOverride("v_separation", separation);
    }

    private void AddMarginAndGridToContainer(PanelContainer container, GMarginContainer margin, GridContainer grid, int separation)
    {
        container.AddChild(margin);
        margin.AddChild(grid);
        margin.SetMarginAll(separation);
    }

    public bool MouseIsOnSlot => _mouseEventManager.MouseIsOnSlot;
    public ItemContainerMouseEventArgs ActiveSlot => _mouseEventManager.ActiveSlot;
}
