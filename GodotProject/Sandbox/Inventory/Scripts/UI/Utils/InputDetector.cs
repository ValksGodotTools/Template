using Godot;
using GodotUtils;

namespace Template.Inventory;

[Service(ServiceLifeTime.Scene)]
public partial class InputDetector : Node2D
{
    public bool HoldingRightClick { get; private set; }
    public bool HoldingLeftClick { get; private set; }
    public bool HoldingShift { get; private set; }

    public override void _Input(InputEvent @event)
    {
        if (@event is InputEventMouseButton mouseBtn)
        {
            HoldingRightClick = mouseBtn.IsRightClickPressed();
            HoldingLeftClick = mouseBtn.IsLeftClickPressed();
        }
        else if (@event is InputEventKey key)
        {
            HoldingShift = key.Keycode == Key.Shift && key.Pressed;
        }
    }
}
