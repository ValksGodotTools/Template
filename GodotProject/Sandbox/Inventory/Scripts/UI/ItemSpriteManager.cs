using Godot;
using System;
using System.Collections.Generic;

namespace Template.Inventory;

public class ItemSpriteManager
{
    private static Dictionary<string, Resource> itemResources = [];

    static ItemSpriteManager()
    {
        itemResources["Coin"] = GD.Load<SpriteFrames>("res://Sandbox/Inventory/CoinSpriteFrames.tres");
        itemResources["Snowy Coin"] = GD.Load<Texture2D>("res://Sandbox/Inventory/CoinStatic.png");
    }

    public static Resource GetResource(Item item)
    {
        if (!itemResources.TryGetValue(item.Name, out Resource resource))
        {
            throw new Exception($"Resource for item '{item.Name}' not found.");
        }

        return resource;
    }
}
