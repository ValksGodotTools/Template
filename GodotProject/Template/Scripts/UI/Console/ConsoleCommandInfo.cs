using CSharpUtils;
using Godot;
using GodotUtils;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

using System.Reflection;

namespace Template;

public class ConsoleCommandInfo
{
    public string Name { get; set; }
    public string[] Aliases { get; set; }
    public MethodInfo Method { get; set; }
}

