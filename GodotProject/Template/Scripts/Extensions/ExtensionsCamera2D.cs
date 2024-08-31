namespace GodotUtils;

using Godot;

public static class ExtensionsCamera2D
{
    public static void SetPositionIgnoreSmoothing(this Camera2D camera, Vector2 position)
    {
        bool smoothEnabled = camera.PositionSmoothingEnabled;

        if (smoothEnabled)
            camera.PositionSmoothingEnabled = false;

        camera.Position = position;

        if (smoothEnabled)
            GTween.Delay(camera, 0.01, () => camera.PositionSmoothingEnabled = true);
    }
}
