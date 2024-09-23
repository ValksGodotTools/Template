using Godot;

namespace Template.Inventory;

public static class InventoryItems
{
    private readonly static SpriteFrames CoinAnimated = GD.Load<SpriteFrames>("res://Sandbox/Inventory/CoinSpriteFrames.tres");
    private readonly static Texture2D CoinStatic = GD.Load<Texture2D>("res://Sandbox/Inventory/CoinStatic.png");

    public static UIInventoryItemSprite YellowCoin()
    {
        return new UIInventoryItemSprite(CoinAnimated).SetColor(Colors.Yellow);
    }

    public static UIInventoryItemSprite SnowyCoin()
    {
        return new UIInventoryItemSprite(CoinStatic).SetColor(Colors.LightBlue);
    }
}
