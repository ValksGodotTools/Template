using Godot;

namespace Template.Inventory;

public class UIInventoryItemSprite : UIContainerBase
{
    private const float DEFAULT_SCALE = 2;
    private readonly DraggableItem _sprite;

    public UIInventoryItemSprite(SpriteFrames spriteFrames) : this()
    {
        InitializeSprite(spriteFrames);
    }

    public UIInventoryItemSprite(Texture2D texture) : this()
    {
        SpriteFrames spriteFrames = new();
        spriteFrames.AddFrame("default", texture);

        InitializeSprite(spriteFrames);
    }

    private UIInventoryItemSprite() : base()
    {
        _sprite = new DraggableItem();
        SetScale(DEFAULT_SCALE);
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

    public override Node Build()
    {
        return _sprite;
    }

    private void InitializeSprite(SpriteFrames spriteFrames)
    {
        _sprite.SpriteFrames = spriteFrames;
        _sprite.Play();
    }
}
