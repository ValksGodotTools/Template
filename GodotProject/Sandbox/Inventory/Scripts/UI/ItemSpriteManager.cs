using Godot;
using System;
using System.Collections.Generic;

namespace Template.Inventory;

public class ItemSpriteManager
{
    private static Dictionary<ItemTexture, ItemVisualData> itemResources = [];

    static ItemSpriteManager()
    {
        itemResources[ItemTexture.CoinAnimated] = new ItemVisualData(
            GD.Load<SpriteFrames>("res://Sandbox/Inventory/CoinSpriteFrames.tres"),
            Colors.Yellow
        );

        itemResources[ItemTexture.CoinStatic] = new ItemVisualData(
            GD.Load<Texture2D>("res://Sandbox/Inventory/CoinStatic.png"),
            Colors.Yellow
        );
    }

    public static ItemVisualData GetResource(Item item)
    {
        if (!itemResources.TryGetValue(item.Texture, out ItemVisualData visualData))
        {
            throw new Exception($"Texture for item '{item.Texture}' not found.");
        }

        return visualData;
    }
}
