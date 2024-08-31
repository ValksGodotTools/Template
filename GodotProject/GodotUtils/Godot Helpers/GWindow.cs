namespace GodotUtils;

using Godot;

public static class GWindow
{
    public static void SetTitle(string title) => DisplayServer.WindowSetTitle(title);

    public static Vector2 GetCenter() => new Vector2(GetWidth() / 2, GetHeight() / 2);
    public static int GetWidth() => DisplayServer.WindowGetSize().X;
    public static int GetHeight() => DisplayServer.WindowGetSize().Y;
}
