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

public partial class UIOptions : PanelContainer
{
    public override void _Ready()
    {
        if (Global.Services.Get<SceneManager>().CurrentScene.Name != "Options")
            GetNode<TextureRect>("BackgroundArt").Hide();
    }
}

