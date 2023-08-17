﻿namespace Template;

using System.Reflection;

public class CommandHelp : Command
{
    static List<string> CommandNames { get; } = Assembly.GetExecutingAssembly()
        .GetTypes()
        .Where(x => typeof(Command).IsAssignableFrom(x) && !x.IsAbstract)
        .Select(Activator.CreateInstance).Cast<Command>()
        .Select(x => x.GetType().Name.Replace("Command", "").ToLower()).ToList();

    public override void Run(Window root, string[] args) => 
        Logger.Log($"Commands:\n{CommandNames.Print()}");
}
