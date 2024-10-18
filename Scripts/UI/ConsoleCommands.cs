using GodotUtils;
using System.Collections.Generic;
using System.Linq;
using Template.Valky;

namespace Template.UI;

public partial class UIConsole
{
    [ConsoleCommand("help")]
    private void Help()
    {
        IEnumerable<string> cmds = Game.Console.Commands.Select(x => x.Name);

        Game.Log(cmds.ToFormattedString());
    }

    [ConsoleCommand("quit", "exit")]
    private async void Quit()
    {
        await GetTree().Root.GetNode<Global>("/root/Global").QuitAndCleanup();
    }

    [ConsoleCommand("debug")]
    private void Debug(int x)
    {
        Game.Log(x);
    }
}

