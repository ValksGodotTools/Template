using Godot;
using GodotUtils;
using Template.TopDown2D;

namespace Template;

public partial class Frog : RigidBody2D
{
    [Export] private FrogResource _config;
    [Export] private AnimatedSprite2D _animatedSprite;
    [Export] private Area2D _area;

    private Player _player;
    private Vector2 _landTarget;

    public override void _Ready()
    {
        Modulate = _config.Color;
    }
}

