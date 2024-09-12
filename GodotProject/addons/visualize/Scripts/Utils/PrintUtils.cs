using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Visualize.Utils;

public static class PrintUtils
{
    public static void Warning(object message)
    {
        GD.PrintRich($"[color=yellow]{message}[/color]");
        GD.PushWarning(message);
    }
}
