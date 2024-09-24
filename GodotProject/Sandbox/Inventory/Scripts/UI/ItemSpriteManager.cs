using Godot;
using System;
using System.Collections.Generic;

namespace Template.Inventory;

public class ItemSpriteManager
{
    private static readonly Dictionary<ItemTexture, ItemVisualData> _itemResources = new()
    {
        { ItemTexture.Coin, new ItemVisualData(GD.Load<SpriteFrames>("res://Sandbox/Inventory/CoinSpriteFrames.tres"), Colors.Yellow) },
        { ItemTexture.CoinSnowy, new ItemVisualData(GD.Load<Texture2D>("res://Sandbox/Inventory/CoinStatic.png"), Colors.LightSkyBlue) }
    };

    public static ItemVisualData GetResource(Item item)
    {
        if (!_itemResources.TryGetValue(item.Texture, out ItemVisualData visualData))
        {
            throw new Exception($"Texture for item '{item.Texture}' not found.");
        }

        return visualData;
    }
}
