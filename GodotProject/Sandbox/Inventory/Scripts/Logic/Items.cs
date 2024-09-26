using Godot;
using System.Collections.Generic;

namespace Template.Inventory;

public static class Items
{
    public static readonly Item Coin = new Item.Builder(nameof(Coin))
        .SetDescription("An ordinary shiny coin.")
        .Build();

    public static readonly Item SnowyCoin = new Item.Builder(nameof(SnowyCoin))
        .SetDescription("A coin with snow infused into it.")
        .Build();

    private static readonly Dictionary<Item, string> ResourcePaths = new()
    {
        { Coin, "res://Sandbox/Inventory/CoinSpriteFrames.tres" },
        { SnowyCoin, "res://Sandbox/Inventory/CoinStatic.png" }
    };

    private static readonly Dictionary<Item, Color> ColorMap = new()
    {
        { Coin, Colors.Yellow },
        { SnowyCoin, Colors.LightSkyBlue }
    };

    public static string GetResourcePath(Item item)
    {
        return ResourcePaths.GetValueOrDefault(item);
    }

    public static Color GetColor(Item item)
    {
        if (ColorMap.TryGetValue(item, out Color color))
        {
            return color;
        }

        return Colors.White;
    }
}
