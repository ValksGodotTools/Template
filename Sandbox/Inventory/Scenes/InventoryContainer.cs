using Godot;

namespace Template.Inventory;

[SceneTree]
public partial class InventoryContainer : PanelContainer
{
    private InventoryInputDetector _inputDetector = new();
    private CanvasLayer _ui;
    private Inventory _inventory;

    [OnInstantiate]
    private void Init(Inventory inventory, int columns = 10)
    {
        GridContainer.Columns = columns;
        _inventory = inventory;
    }

    public override void _Ready()
    {
        _ui = GetTree().CurrentScene.GetNode<CanvasLayer>("%UI");

        AddItemContainers(_inventory);
    }

    public override void _Input(InputEvent @event)
    {
        _inputDetector.Update(@event);
    }

    private void AddItemContainers(Inventory inventory)
    {
        ItemContainer[] itemContainers = new ItemContainer[inventory.GetItemSlotCount()];

        InventoryVFXContext vfxContext = new(_ui, itemContainers, inventory);
        InventoryVFXManager vfxManager = new();
        InventoryInputHandler inputHandler = new(_inputDetector);

        vfxManager.RegisterEvents(inputHandler, vfxContext, this);
        inputHandler.RegisterInput(vfxContext);

        for (int i = 0; i < itemContainers.Length; i++)
        {
            ItemContainer itemContainer = AddItemContainer();
            itemContainer.SetItem(inventory.GetItem(i));
            itemContainers[i] = itemContainer;

            int index = i; // Capture i

            itemContainer.GuiInput += @event =>
            {
                inputHandler.HandleGuiInput(@event, vfxContext, index);
            };

            itemContainer.MouseEntered += () =>
            {
                inputHandler.HandleMouseEntered(vfxContext, vfxManager, index, GetGlobalMousePosition());
            };
        }

        inventory.OnItemChanged += (index, item) =>
        {
            itemContainers[index].SetItem(item);
        };
    }

    private ItemContainer AddItemContainer()
    {
        ItemContainer itemContainer = ItemContainer.Instantiate();
        GridContainer.AddChild(itemContainer);
        return itemContainer;
    }
}
