namespace Template;

public partial class UIConsole
{
    [ConsoleCommand("help")]
    void Help()
    {
        IEnumerable<string> cmds =
            Global.Services.Get<UIConsole>().Commands.Select(x => x.Name);

        Global.Services.Get<Logger>().Log(cmds.Print());
    }

    [ConsoleCommand("quit", "exit")]
    void Quit()
    {
        GetTree().Root.GetNode<Global>("/root/Global").Quit();
    }

    [ConsoleCommand("debug")]
    void Debug(int x)
    {
        Global.Services.Get<Logger>().Log(x);
    }
}
