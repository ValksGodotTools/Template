using Godot;
using System;
using System.Collections.Generic;

namespace Template.Inventory;

public class ItemSpriteManager
{
    private static Dictionary<ItemTexture, Resource> itemResources = [];

    static ItemSpriteManager()
    {
        itemResources[ItemTexture.CoinAnimated] = GD.Load<SpriteFrames>("res://Sandbox/Inventory/CoinSpriteFrames.tres");
        itemResources[ItemTexture.CoinStatic] = GD.Load<Texture2D>("res://Sandbox/Inventory/CoinStatic.png");
    }

    public static Resource GetResource(Item item)
    {
        if (!itemResources.TryGetValue(item.Texture, out Resource resource))
        {
            throw new Exception($"Texture for item '{item.Texture}' not found.");
        }

        return resource;
    }
}
