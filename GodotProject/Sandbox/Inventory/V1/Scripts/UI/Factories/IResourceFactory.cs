using Godot;

namespace Template.InventoryV1;

public interface IResourceFactory
{
    InventoryItemSprite CreateSprite(Resource resource, InventoryItemContainer itemContainer);
}
