using Godot;

namespace Template;

public static class PrintUtils
{
    public static void Warning(object message)
    {
        GD.PrintRich($"[color=yellow]{message}[/color]");
        GD.PushWarning(message);
    }
}
