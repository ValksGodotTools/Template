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

public static class Utils
{
    public static float ClampAndDampen(float horzVelocity, float dampening, float maxSpeedGround)
    {
        return Mathf.Abs(horzVelocity) <= dampening ? 
            0 : horzVelocity > 0 ?
                Mathf.Min(horzVelocity - dampening, maxSpeedGround) :
                Mathf.Max(horzVelocity + dampening, -maxSpeedGround);
    }
}

