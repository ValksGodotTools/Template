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

public partial class Env : WorldEnvironment
{
    [Export] OptionsManager options;

    public override void _Ready()
    {
        ResourceOptions options = this.options.Options;
        
        Environment.GlowEnabled = options.Glow;
        Environment.SsrEnabled = options.Reflections;
        Environment.SsaoEnabled = options.AmbientOcclusion;
        Environment.SsilEnabled = options.IndirectLighting;
    }
}

