using Godot;

namespace GodotUtils;

public static class GInput
{
    public static bool IsMovingLeft()
    {
        return Input.IsKeyPressed(Key.Left) || Input.IsKeyPressed(Key.A);
    }

    public static bool IsMovingRight()
    {
        return Input.IsKeyPressed(Key.Right) || Input.IsKeyPressed(Key.D);
    }

    public static bool IsMovingUp()
    {
        return Input.IsKeyPressed(Key.Up) || Input.IsKeyPressed(Key.W);
    }

    public static bool IsMovingDown()
    {
        return Input.IsKeyPressed(Key.Down) || Input.IsKeyPressed(Key.S);
    }
}

