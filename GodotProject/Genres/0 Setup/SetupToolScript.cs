using CSharpUtils;
using Godot;
using GodotUtils;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Template;

[Tool]
public partial class SetupToolScript : Node
{
    [Export] public bool RemoveEmptyFolders
    {
        get => false;
        set
        {
            GDirectories.DeleteEmptyDirectories(ProjectSettings.GlobalizePath("res://"));
            GD.Print("Removed all empty folders from the project");
        }
    }
}

