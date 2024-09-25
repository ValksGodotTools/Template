using Godot;

namespace Template.Inventory;

public interface IInventoryInput
{
    void Handle(InventorySlotContext context);
    bool CheckInput(InputEventMouseButton mouseBtn);
}
