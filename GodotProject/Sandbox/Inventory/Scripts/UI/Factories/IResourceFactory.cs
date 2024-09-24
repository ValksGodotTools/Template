using Godot;

namespace Template.Inventory;

public interface IResourceFactory
{
    InventoryItemSprite CreateSprite(Resource resource, InventoryItemContainer itemContainer);
}
