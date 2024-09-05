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

public partial class UIMainMenu : Node
{
    [Export] GameState gameState;

    public override void _Ready()
    {
        //Global.Services.Get<AudioManager>().PlayMusic(Music.Menu);
    }
}

