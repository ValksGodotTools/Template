using CSharpUtils;
using Godot;
using GodotUtils;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Template.FPS3D;

public partial class Player : CharacterBody3D
{   
    event Action OnFinishedReload;

    public override void _Ready()
    {
        OnReadyUI();
        OnReadyAnimation();
    }

    public override void _PhysicsProcess(double delta)
    {
        OnPhysicsProcessUI();
        OnPhysicsProcessMotion(delta);
        OnPhysicsProcessAnimation();
    }

    public override void _Input(InputEvent @event)
    {
        OnInputUI(@event);
    }
}

