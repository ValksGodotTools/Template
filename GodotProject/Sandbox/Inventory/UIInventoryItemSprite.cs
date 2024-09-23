using Godot;
using GodotUtils;

namespace Template.Inventory;

public class UIInventoryItemSprite : UIContainerBase
{
    private readonly UIInventoryDraggableAnimatedSprite _sprite;

    public UIInventoryItemSprite(SpriteFrames spriteFrames)
    {
        _sprite = new UIInventoryDraggableAnimatedSprite();
        _sprite.SpriteFrames = spriteFrames;
        _sprite.Play();
    }

    public UIInventoryItemSprite SetColor(Color color)
    {
        _sprite.SelfModulate = color;
        return this;
    }

    public UIInventoryItemSprite SetScale(float scale)
    {
        _sprite.Scale = Vector2.One * scale;
        return this;
    }

    public Vector2 GetSize()
    {
        return _sprite.GetScaledSize();
    }

    public override Node Build()
    {
        return _sprite;
    }
}
