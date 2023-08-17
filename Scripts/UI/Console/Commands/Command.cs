namespace GodotUtils;

using System.Reflection;

/*
 * Create new Command classes that extend from this to create new commands for
 * use in the UIConsole. Make sure all command classes have CommandName formatting
 * for the name.
 */
public abstract class Command
{
    public string[] Aliases { get; set; }

    public bool IsMatch(string cmd)
    {
        var cmdMatchesAlias = false;
        if (Aliases != null)
            cmdMatchesAlias = Aliases.Contains(cmd);

        return cmdMatchesAlias || GetType().Name.Replace("Command", "").ToLower() == cmd;
    }

    public abstract void Run(Window root, string[] args);
}
