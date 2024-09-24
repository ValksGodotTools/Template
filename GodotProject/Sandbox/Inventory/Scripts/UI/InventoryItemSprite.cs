using Godot;

namespace Template.Inventory;

public class InventoryItemSprite
{
    public DraggableItem DraggableItem { get; private set; }

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
        DraggableItem = new DraggableItem();
        DraggableItem.InventoryItemContainer = itemContainer;

        const float DEFAULT_SCALE = 2;

        SetScale(DEFAULT_SCALE);
    }

    public void SetCount(int count)
    {
        DraggableItem.SetItemCount(count);
    }

    public void SetColor(Color color)
    {
        DraggableItem.SelfModulate = color;
    }

    public void SetScale(float scale)
    {
        DraggableItem.Scale = Vector2.One * scale;
    }

    public DraggableItem Build()
    {
        return DraggableItem;
    }

    private void InitializeSprite(SpriteFrames spriteFrames)
    {
        DraggableItem.SpriteFrames = spriteFrames;
        DraggableItem.Play();
    }
}
