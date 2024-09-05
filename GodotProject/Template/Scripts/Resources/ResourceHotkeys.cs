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

using Godot.Collections;

public partial class ResourceHotkeys : Resource
{
    [Export] public Dictionary<StringName, Array<InputEvent>> Actions { get; set; } = new();
}

