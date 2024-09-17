using Godot;

namespace Template.TopDown2D;

public partial class Frog : RigidBody2D
{
    [Export] private FrogResource _config;

    public override void _Ready()
    {
        Modulate = _config.Color;
    }
}

