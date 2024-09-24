using Godot;
using System;
using System.Collections.Generic;

namespace Template.Inventory;

public class ResourceFactoryRegistry
{
    private static readonly Dictionary<Type, IResourceFactory> _factories = new()
    {
        [typeof(SpriteFrames)] = new SpriteFramesFactory(),
        [typeof(CompressedTexture2D)] = new Texture2DFactory()
    };

    public static InventoryItemSprite CreateSprite(ItemVisualData itemVisualData)
    {
        Type resourceType = itemVisualData.Resource.GetType();

        if (_factories.TryGetValue(resourceType, out IResourceFactory factory))
        {
            InventoryItemSprite sprite = factory.CreateSprite(itemVisualData.Resource);
            sprite.SetColor(itemVisualData.Color);

            return sprite;
        }

        throw new ArgumentException($"No factory found for resource type {resourceType}.");
    }
}
