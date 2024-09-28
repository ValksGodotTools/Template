using Godot;
using System;
using System.Collections.Generic;

namespace Template.Inventory;

public static partial class ItemInformation
{
    private static readonly Dictionary<Material, Item> _items;

    static ItemInformation()
    {
        _items = new()
        {
            { Material.Coin, Coin },
            { Material.SnowyCoin, SnowyCoin }
        };
    }

    public static Item Get(Material material)
    {
        if (_items.TryGetValue(material, out Item item))
        {
            return item;
        }

        throw new NotImplementedException(nameof(material));
    }

    private static readonly Item Coin = new Item(nameof(Coin))
        .SetDescription("An ordinary shiny coin.")
        .SetResource("res://Sandbox/Inventory/CoinSpriteFrames.tres")
        .SetColor(Colors.Yellow);

    private static readonly Item SnowyCoin = new Item(nameof(SnowyCoin))
        .SetDescription("A coin with snow infused into it.")
        .SetResource("res://Sandbox/Inventory/CoinStatic.png")
        .SetColor(Colors.LightSkyBlue);
}
