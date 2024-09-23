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

    public static UIInventoryItemSprite CreateSprite(Resource resource)
    {
        Type resourceType = resource.GetType();

        if (_factories.TryGetValue(resourceType, out IResourceFactory factory))
        {
            return factory.CreateSprite(resource);
        }

        throw new ArgumentException($"No factory found for resource type {resourceType}.");
    }
}
