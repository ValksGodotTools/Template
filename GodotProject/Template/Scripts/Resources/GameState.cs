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

[GlobalClass]
public partial class GameState : Resource
{
    [Export] OptionsManager optionsManager;

    public GameState()
    {
        
    }
}

