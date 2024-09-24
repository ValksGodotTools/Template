using Godot;
using System;

namespace Template.Inventory;

public class Texture2DFactory : IResourceFactory
{
    public InventoryItemSprite CreateSprite(Resource resource)
    {
        if (resource is Texture2D texture)
        {
            return new InventoryItemSprite(texture);
        }

        throw new ArgumentException("Resource is not a Texture2D instance.");
    }
}
