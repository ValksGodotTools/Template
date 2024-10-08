using Godot;

namespace Template.Unorganized;

public static class Utils
{
    public static Vector2 GetMovementInput(string prefix = "")
    {
        prefix += !string.IsNullOrWhiteSpace(prefix) ? "_" : "";

        return Input.GetVector(
            $"{prefix}move_left", $"{prefix}move_right",
            $"{prefix}move_up", $"{prefix}move_down");
    }

    public static Vector2 GetMovementInputRaw(string prefix = "")
    {
        return GetMovementInput(prefix).Round();
    }
}

