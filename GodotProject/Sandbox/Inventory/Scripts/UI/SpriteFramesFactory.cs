using Godot;
using System;

namespace Template.Inventory;

public class SpriteFramesFactory : IResourceFactory
{
    public UIInventoryItemSprite CreateSprite(Resource resource)
    {
        if (resource is SpriteFrames spriteFrames)
        {
            return new UIInventoryItemSprite(spriteFrames);
        }

        throw new ArgumentException("Resource is not a SpriteFrames instance.");
    }
}
