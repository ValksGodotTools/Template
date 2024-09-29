using Godot;

namespace GodotUtils;

public static class InputEventKeyExtensions
{
    public static bool IsJustPressed(this InputEventKey v, Key key)
    {
        return v.Keycode == key && v.Pressed && !v.Echo;
    }

    public static bool IsJustReleased(this InputEventKey v, Key key)
    {
        return v.Keycode == key && !v.Pressed && !v.Echo;
    }

    /// <summary>
    /// <para>Convert to a human readable key</para>
    /// <para>For example 'Ctrl + Shift + E'</para>
    /// </summary>
    public static string Readable(this InputEventKey v)
    {
        // If Keycode is not set than use PhysicalKeycode
        Key keyWithModifiers = v.Keycode == Key.None ?
            v.GetPhysicalKeycodeWithModifiers() :
            v.GetKeycodeWithModifiers();

        return OS.GetKeycodeString(keyWithModifiers).Replace("+", " + ");
    }
}

