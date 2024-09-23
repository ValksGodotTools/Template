using Godot;
using System;

namespace Template.Inventory;

public class Texture2DFactory : IResourceFactory
{
    public UIInventoryItemSprite CreateSprite(Resource resource)
    {
        if (resource is Texture2D texture)
        {
            return new UIInventoryItemSprite(texture);
        }

        throw new ArgumentException("Resource is not a Texture2D instance.");
    }
}
