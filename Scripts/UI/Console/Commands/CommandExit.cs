namespace Template;

public class CommandExit : Command
{
    public CommandExit() => Aliases = new[] { "quit" };

    public override void Run(Window root, string[] args)
    {
        root.GetNode<Global>("/root/Global").Quit();
    }
}
