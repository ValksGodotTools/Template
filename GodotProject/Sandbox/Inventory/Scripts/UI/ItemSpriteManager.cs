using Godot;
using System;
using System.Collections.Generic;

namespace Template.Inventory;

public class ItemSpriteManager
{
    private Dictionary<string, Resource> itemResources = [];

    public ItemSpriteManager()
    {
        LoadItemResources();
    }

    private void LoadItemResources()
    {
        itemResources["Coin"] = GD.Load<SpriteFrames>("res://Sandbox/Inventory/CoinSpriteFrames.tres");
        itemResources["Snowy Coin"] = GD.Load<Texture2D>("res://Sandbox/Inventory/CoinStatic.png");
    }

    public Resource GetResource(Item item)
    {
        if (!itemResources.TryGetValue(item.Name, out Resource resource))
        {
            throw new Exception($"Resource for item '{item.Name}' not found.");
        }

        return resource;
    }
}
