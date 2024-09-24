using Godot;
using System;

namespace Template.InventoryV1;

public class Texture2DFactory : IResourceFactory
{
    public InventoryItemSprite CreateSprite(Resource resource, InventoryItemContainer itemContainer)
    {
        if (resource is Texture2D texture)
        {
            return new InventoryItemSprite(texture, itemContainer);
        }

        throw new ArgumentException("Resource is not a Texture2D instance.");
    }
}
