using CSharpUtils;
using Godot;
using GodotUtils;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Template.Platformer2D;

public partial class Player
{
    State Slide()
    {
        State state = new(nameof(Slide));

        state.Enter = () =>
        {

        };

        state.Update = delta =>
        {

        };

        state.Transitions = () =>
        {

        };

        return state;
    }
}

