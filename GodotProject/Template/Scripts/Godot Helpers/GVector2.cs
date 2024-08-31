namespace GodotUtils;

using Godot;

public static class GVector2
{
    /// <summary>
    /// Returns a random vector between 0 and 1 (inclusive) for X and Y.
    /// </summary>
    public static Vector2 Random() => 
        new Vector2(GMath.RandRange(-1.0, 1.0), GMath.RandRange(-1.0, 1.0)).Normalized();
}
