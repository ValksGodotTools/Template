namespace Template;

public partial class UIConsole
{
    [ConsoleCommand("help")]
    void Help()
    {
        IEnumerable<string> cmds =
            UIConsole.Instance.Commands.Select(x => x.Name);

        Global.Services.Get<Logger>().Log(cmds.Print());
    }

    [ConsoleCommand("quit", "exit")]
    void Quit()
    {
        GetTree().Root.GetNode<Global>("/root/Global").Quit();
    }

    [ConsoleCommand("debug")]
    void Debug()
    {
        Global.Services.Get<Logger>().Log("Debug");
    }
}
