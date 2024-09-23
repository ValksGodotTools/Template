using Godot;

namespace Template.Inventory;

public static class InventoryItems
{
    public static UIInventoryItemSprite YellowCoin()
    {
        SpriteFrames spriteFrames = GD.Load<SpriteFrames>("res://Sandbox/Inventory/CoinSpriteFrames.tres");

        return new UIInventoryItemSprite(spriteFrames)
            .SetColor(Colors.Yellow)
            .SetScale(2);
    }

    public static UIInventoryItemSprite SnowyCoin()
    {
        Texture2D texture = GD.Load<Texture2D>("res://Sandbox/Inventory/CoinStatic.png");

        return new UIInventoryItemSprite(texture)
            .SetColor(Colors.LightBlue)
            .SetScale(2);
    }
}
