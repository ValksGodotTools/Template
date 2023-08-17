namespace Template;

using System.Reflection;

public class CommandHelp : Command
{
    // This must be static or the game will freeze on startup
    // Not sure if this is a Godot bug or not
    static List<string> CommandNames = Assembly.GetExecutingAssembly()
        .GetTypes()
        .Where(x => typeof(Command).IsAssignableFrom(x) && !x.IsAbstract)
        .Select(Activator.CreateInstance).Cast<Command>()
        .Select(x => x.GetType().Name.Replace("Command", "")
        .ToLower())
        .ToList();

    public override void Run(Window root, string[] args) => 
        Logger.Log($"Commands:\n{CommandNames.Print()}");
}
