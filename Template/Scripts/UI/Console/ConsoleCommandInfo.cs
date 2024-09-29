using System.Reflection;

namespace Template;

public class ConsoleCommandInfo
{
    public string Name { get; set; }
    public string[] Aliases { get; set; }
    public MethodInfo Method { get; set; }
}

