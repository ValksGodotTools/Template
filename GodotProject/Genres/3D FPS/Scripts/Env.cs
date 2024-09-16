using Godot;
using System;

namespace Template;

public partial class Env : WorldEnvironment
{
    [Export] private OptionsManager options;

    public override void _Ready()
    {
        ResourceOptions options = this.options.Options;
        
        Environment.GlowEnabled = options.Glow;
        Environment.SsrEnabled = options.Reflections;
        Environment.SsaoEnabled = options.AmbientOcclusion;
        Environment.SsilEnabled = options.IndirectLighting;
    }
}

