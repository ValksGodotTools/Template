using Godot;

namespace Template.Inventory;

public class InventoryItemSprite
{
    private const float DEFAULT_SCALE = 2;
    private readonly DraggableItem _item;

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

    private InventoryItemSprite()
    {
        _item = new DraggableItem();
        SetScale(DEFAULT_SCALE);
    }

    public void SetCount(int count)
    {
        _item.SetItemCount(count);
    }

    public void SetColor(Color color)
    {
        _item.SelfModulate = color;
    }

    public void SetScale(float scale)
    {
        _item.Scale = Vector2.One * scale;
    }

    public DraggableItem Build()
    {
        return _item;
    }

    private void InitializeSprite(SpriteFrames spriteFrames)
    {
        _item.SpriteFrames = spriteFrames;
        _item.Play();
    }
}
