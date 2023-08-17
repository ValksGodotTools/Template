namespace Template;

public class CommandDebug : Command
{
    public override void Run(Window root, string[] args)
    {
        Logger.Log("Debug");
    }
}
