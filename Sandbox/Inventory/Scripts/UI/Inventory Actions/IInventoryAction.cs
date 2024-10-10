using Godot;

namespace Template.Inventory;

public interface IInventoryAction
{
    void Execute(InventoryContext context, MouseButton mouseBtn, int index);
}
