using CSharpUtils;
using Godot;
using GodotUtils;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Template;

public partial class UIConsole
{
    [ConsoleCommand("help")]
    void Help()
    {
        IEnumerable<string> cmds = Game.Console.Commands.Select(x => x.Name);

        Game.Log(cmds.Print());
    }

    [ConsoleCommand("quit", "exit")]
    async void Quit()
    {
        await GetTree().Root.GetNode<Global>("/root/Global").QuitAndCleanup();
    }

    [ConsoleCommand("debug")]
    void Debug(int x)
    {
        Game.Log(x);
    }
}

