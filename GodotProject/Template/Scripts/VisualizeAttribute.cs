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

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Method)]
public class VisualizeAttribute : Attribute
{
    public Vector2 InitialPosition { get; }

    public VisualizeAttribute()
    {
        InitialPosition = Vector2.Zero;
    }

    public VisualizeAttribute(float x, float y)
    {
        InitialPosition = new Vector2(x, y);
    }
}

