using Godot;

namespace Template.TopDown2D;

public static class PlayerUtils
{
    public static Vector2 GetDirection()
    {
        return Input.GetVector(InputActions.MoveLeft, InputActions.MoveRight, InputActions.MoveUp, InputActions.MoveDown);
    }
}

