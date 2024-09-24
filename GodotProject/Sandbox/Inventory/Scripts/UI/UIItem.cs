using Godot;
using GodotUtils;

namespace Template.Inventory;

[Draggable]
public partial class UIItem : AnimatedSprite2D, IDraggable
{
    public int Count { get; set; }

    private InventoryItemContainer _inventoryItemContainer;
    private Label _itemCountLabel;

    public override void _Ready()
    {
        _itemCountLabel = CreateItemCountLabel();
        AddChild(_itemCountLabel);
    }

    public void SetInventoryItemContainer(InventoryItemContainer container)
    {
        _inventoryItemContainer = container;
    }

    public void SetItemCount(int count)
    {
        Count = count;
        _itemCountLabel.Text = Count.ToString();
    }

    public void OnDragReleased()
    {
        if (_inventoryItemContainer.InventoryContainer.MouseIsOnSlot)
        {
            ItemContainerMouseEventArgs otherSlot = _inventoryItemContainer.InventoryContainer.ActiveSlot;

            _inventoryItemContainer.SwapItems(otherSlot.InventoryItemContainer);

            QueueFree();
        }
    }

    private Label CreateItemCountLabel()
    {
        Vector2 size = this.GetSize();

        Label label = new()
        {
            Text = "0",
            Scale = Vector2.One * 0.25f,
            Position = size * 0.1f + new Vector2(size.X * 0.3f, 0),
            LabelSettings = new LabelSettings
            {
                FontSize = 32,
            },
        };

        return label;
    }
}
