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
        bool cmdMatchesAlias = false;
        if (Aliases != null)
            cmdMatchesAlias = Aliases.Contains(cmd);

        bool cmdMatchesType = 
            GetType().Name.Replace("Command", "").ToLower() == cmd;

        return cmdMatchesAlias || cmdMatchesType;
    }

    public abstract void Run(Window root, string[] args);
}
