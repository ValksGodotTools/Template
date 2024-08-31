namespace GodotUtils;

using Godot;

public static class GInput
{
    public static bool IsMovingLeft() =>
        Input.IsKeyPressed(Key.Left) || Input.IsKeyPressed(Key.A);
    public static bool IsMovingRight() =>
        Input.IsKeyPressed(Key.Right) || Input.IsKeyPressed(Key.D);
    public static bool IsMovingUp() =>
        Input.IsKeyPressed(Key.Up) || Input.IsKeyPressed(Key.W);
    public static bool IsMovingDown() =>
        Input.IsKeyPressed(Key.Down) || Input.IsKeyPressed(Key.S);
}
