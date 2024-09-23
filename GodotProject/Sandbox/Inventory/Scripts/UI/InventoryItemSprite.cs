using Godot;

namespace Template.Inventory;

public class InventoryItemSprite : ContainerBase
{
    private const float DEFAULT_SCALE = 2;
    private readonly DraggableItem _sprite;

    public InventoryItemSprite(SpriteFrames spriteFrames) : this()
    {
        InitializeSprite(spriteFrames);
    }

    public InventoryItemSprite(Texture2D texture) : this()
    {
        SpriteFrames spriteFrames = new();
        spriteFrames.AddFrame("default", texture);

        InitializeSprite(spriteFrames);
    }

    private InventoryItemSprite() : base()
    {
        _sprite = new DraggableItem();
        SetScale(DEFAULT_SCALE);
    }

    public InventoryItemSprite SetColor(Color color)
    {
        _sprite.SelfModulate = color;
        return this;
    }

    public InventoryItemSprite SetScale(float scale)
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
