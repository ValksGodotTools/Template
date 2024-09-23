using Godot;

namespace Template.Inventory;

public interface IResourceFactory
{
    UIInventoryItemSprite CreateSprite(Resource resource);
}
