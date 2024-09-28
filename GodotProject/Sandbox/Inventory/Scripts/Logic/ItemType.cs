using Godot;

namespace Template.Inventory;

public static class ItemType
{
    public static readonly Item Coin = new Item(nameof(Coin))
        .SetDescription("An ordinary shiny coin.")
        .SetResource("res://Sandbox/Inventory/CoinSpriteFrames.tres")
        .SetColor(Colors.Yellow);

    public static readonly Item SnowyCoin = new Item(nameof(SnowyCoin))
        .SetDescription("A coin with snow infused into it.")
        .SetResource("res://Sandbox/Inventory/CoinStatic.png")
        .SetColor(Colors.LightSkyBlue);
}
