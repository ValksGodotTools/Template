using Godot;
using GodotUtils;
using System.Drawing;

namespace Template.Inventory;

[Draggable]
public partial class DraggableItem : AnimatedSprite2D, IDraggable
{
    private Label _itemCountLabel;

    public override void _Ready()
    {
        _itemCountLabel = CreateItemCountLabel();
        AddChild(_itemCountLabel);
    }

    public void SetItemCount(int count)
    {
        _itemCountLabel.Text = count.ToString();
    }

    public void OnDragReleased()
    {
        Area2D area = CursorUtils2D.GetAreaUnder(this);

        DraggableItem otherItem = area?.GetParent<DraggableItem>();

        GD.Print(otherItem);
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
