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

public static class Sounds
{
    public static AudioStream GameOver { get; } =
        Load("Game Over/musical-game-over.wav");

    static AudioStream Load(string path) =>
        GD.Load<AudioStream>($"res://Audio/SFX/{path}");
}

