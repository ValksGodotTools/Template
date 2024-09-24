using Godot;
using GodotUtils;

namespace Template.Inventory;

[Draggable]
public partial class DraggableItem : AnimatedSprite2D, IDraggable
{
    public int Count { get; set; }
    public InventoryItemContainer InventoryItemContainer { get; set; }

    private Label _itemCountLabel;

    public override void _Ready()
    {
        _itemCountLabel = CreateItemCountLabel();
        AddChild(_itemCountLabel);
    }

    public void SetItemCount(int count)
    {
        Count = count;
        _itemCountLabel.Text = Count.ToString();
    }

    public void OnDragReleased()
    {
        InventoryItemContainer itemContainer = InventoryItemContainer;
        InventoryContainer inventoryContainer = itemContainer.InventoryContainer;

        if (inventoryContainer.MouseIsOnSlot)
        {
            ItemContainerMouseEventArgs otherSlot = inventoryContainer.ActiveSlot;
            InventoryItemContainer otherItemContainer = otherSlot.InventoryItemContainer;
            DraggableItem otherItem = otherItemContainer.DraggableItem;

            if (otherItem == null)
            {
                otherItemContainer.SetItem(itemContainer.Item);
                QueueFree();
            }
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
