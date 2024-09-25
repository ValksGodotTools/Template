using Godot;
using GodotUtils;

namespace Template.Inventory;

public class InventoryInputRightClick : IInventoryInput
{
    public bool CheckInput(InputEventMouseButton mouseBtn)
    {
        return mouseBtn.IsRightClickPressed();
    }

    public void Handle(InventorySlotContext context)
    {
        
    }
}
