using Godot;
using GodotUtils;

namespace Template.Inventory;

[Draggable]
public partial class UIItem : AnimatedSprite2D, IDraggable
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
        InventoryItemContainer thisInvItemContainer = InventoryItemContainer;
        InventoryContainer thisInvContainer = thisInvItemContainer.InventoryContainer;
        Inventory thisInv = thisInvContainer.Inventory;

        if (thisInvContainer.MouseIsOnSlot)
        {
            ItemContainerMouseEventArgs otherSlot = thisInvContainer.ActiveSlot;

            InventoryItemContainer otherInvItemContainer = otherSlot.InventoryItemContainer;
            InventoryContainer otherInvContainer = otherInvItemContainer.InventoryContainer;
            Inventory otherInv = otherInvContainer.Inventory;

            UIItem otherItem = otherInvItemContainer.UIItem;

            // Dragged an item onto an empty inventory slot
            if (otherItem == null)
            {
                otherInvItemContainer.SetItem(thisInv.GetItem(thisInvItemContainer.Index));
                thisInv.SwapItems(thisInvItemContainer.Index, otherInvItemContainer.Index);

                otherInvItemContainer.UIItem = this;
                thisInvItemContainer.UIItem = null;

                QueueFree();
            }
            // Dragged an item onto an inventory slot that has an item here
            else
            {
                
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
