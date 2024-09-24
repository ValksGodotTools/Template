using Godot;

namespace Template.InventoryV2;

public static class Items
{
    public static readonly Item Coin = new(nameof(Coin))
    {
        ResourcePath = "res://Sandbox/Inventory/CoinSpriteFrames.tres",
        Color = Colors.Yellow
    };

    public static readonly Item SnowyCoin = new(nameof(SnowyCoin))
    {
        ResourcePath = "res://Sandbox/Inventory/CoinStatic.png",
        Color = Colors.LightSkyBlue
    };
}
