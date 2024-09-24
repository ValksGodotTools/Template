using Godot;

namespace Template.InventoryV1;

public class InventoryItemSprite
{
    public UIItem UIItem { get; private set; }

    public InventoryItemSprite(SpriteFrames spriteFrames, InventoryItemContainer itemContainer) : this(itemContainer)
    {
        InitializeSprite(spriteFrames);
    }

    public InventoryItemSprite(Texture2D texture, InventoryItemContainer itemContainer) : this(itemContainer)
    {
        SpriteFrames spriteFrames = new();
        spriteFrames.AddFrame("default", texture);
        InitializeSprite(spriteFrames);
    }

    private InventoryItemSprite(InventoryItemContainer itemContainer)
    {
        UIItem = new UIItem();
        UIItem.SetInventoryItemContainer(itemContainer);
        SetScale(2);
    }

    public void SetCount(int count)
    {
        UIItem.SetItemCount(count);
    }

    public void SetColor(Color color)
    {
        UIItem.SelfModulate = color;
    }

    public void SetScale(float scale)
    {
        UIItem.Scale = Vector2.One * scale;
    }

    public UIItem Build()
    {
        return UIItem;
    }

    private void InitializeSprite(SpriteFrames spriteFrames)
    {
        UIItem.SpriteFrames = spriteFrames;
        UIItem.Play();
    }
}
