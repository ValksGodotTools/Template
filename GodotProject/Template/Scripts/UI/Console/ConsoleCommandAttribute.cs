using System;


namespace Template;

[AttributeUsage(AttributeTargets.Method)]
public class ConsoleCommandAttribute : Attribute
{
    public string Name { get; set; }
    public string Description { get; set; }
    public string[] Aliases { get; set; }

    public ConsoleCommandAttribute(string name, params string[] aliases)
    {
        Name = name;
        Aliases = aliases;
    }
}

