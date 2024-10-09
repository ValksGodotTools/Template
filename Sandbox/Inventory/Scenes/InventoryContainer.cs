using Godot;

namespace Template.Inventory;

[SceneTree]
public partial class InventoryContainer : PanelContainer
{
    public Inventory Inventory { get; private set; }
    public ItemContainer[] ItemContainers { get; private set; }

    private InventoryInputDetector _inputDetector = new();
    private CanvasLayer _ui;

    [OnInstantiate]
    private void Init(Inventory inventory, int columns = 10)
    {
        GridContainer.Columns = columns;
        Inventory = inventory;
    }

    public override void _Ready()
    {
        _ui = GetTree().CurrentScene.GetNode<CanvasLayer>("%UI");

        AddItemContainers(Inventory);
    }

    public override void _Input(InputEvent @event)
    {
        _inputDetector.Update(@event);
    }

    private void AddItemContainers(Inventory inventory)
    {
        ItemContainers = new ItemContainer[inventory.GetItemSlotCount()];

        InventoryVFXContext vfxContext = new(_ui, ItemContainers, inventory);
        InventoryVFXManager vfxManager = new();
        InventoryInputHandler inputHandler = new(_inputDetector);

        vfxManager.RegisterEvents(inputHandler, vfxContext, this);
        inputHandler.RegisterInput(this, vfxContext);

        for (int i = 0; i < ItemContainers.Length; i++)
        {
            ItemContainer itemContainer = AddItemContainer();
            itemContainer.SetItem(inventory.GetItem(i));
            ItemContainers[i] = itemContainer;

            int index = i; // Capture i

            itemContainer.GuiInput += @event =>
            {
                inputHandler.HandleGuiInput(this, @event, vfxContext, index);
            };

            itemContainer.MouseEntered += () =>
            {
                inputHandler.HandleMouseEntered(this, vfxContext, vfxManager, index, GetGlobalMousePosition());
            };
        }

        inventory.OnItemChanged += (index, item) =>
        {
            ItemContainers[index].SetItem(item);
        };
    }

    private ItemContainer AddItemContainer()
    {
        ItemContainer itemContainer = ItemContainer.Instantiate();
        GridContainer.AddChild(itemContainer);
        return itemContainer;
    }
}
