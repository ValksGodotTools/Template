using Godot;
using GodotUtils;
using System;

namespace Template.TopDown2D;

/// <summary>
/// This script assumes all enemies are of type Node2D and have animated sprite nodes.
/// </summary>
[GlobalClass, Icon("res://Template/Sprites/Icons/Gear/gear.svg")]
public partial class EnemyComponent : EntityComponent
{
    [Export] public StateMachineComponent StateMachine { get; private set; }
    [Export] private Area2D _hitbox;

    public Node2D Target { get; set; }

    public override void _Ready()
    {
        base._Ready();
        ServiceProvider.Services.Get<Level>().EnemyComponents.Add(this);

        _hitbox.BodyEntered += body =>
        {
            if (body.HasNode<PlayerComponent>())
            {
                GD.Print("We are a player!");
            }
        };
    }
}
