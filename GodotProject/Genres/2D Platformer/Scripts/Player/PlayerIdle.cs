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
    State Idle()
    {
        State state = new(nameof(Idle));

        state.Enter = () =>
        {
            sprite.Play("idle");
        };

        state.Transitions = () =>
        {
            if (Input.IsActionJustPressed("jump") && IsOnFloor())
            {
                SwitchState(Jump());
            }

            else if (Input.IsActionJustPressed("move_down") && GetFloorAngle() > 0)
            {

            }
        };

        return state;
    }
}

