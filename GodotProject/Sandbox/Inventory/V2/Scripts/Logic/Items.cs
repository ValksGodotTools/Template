using Godot;

namespace Template.InventoryV2;

public static class Items
{
    public static readonly Item Coin = new Item.Builder(nameof(Coin))
        .SetResourcePath("CoinSpriteFrames.tres")
        .SetColor(Colors.Yellow)
        .Build();

    public static readonly Item SnowyCoin = new Item.Builder(nameof(SnowyCoin))
        .SetResourcePath("CoinStatic.png")
        .SetColor(Colors.LightSkyBlue)
        .Build();
}
