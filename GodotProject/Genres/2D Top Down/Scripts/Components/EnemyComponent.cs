using Godot;

namespace Template.TopDown2D;

/// <summary>
/// This script assumes all enemies are of type Node2D and have animated sprite nodes.
/// </summary>
[GlobalClass, Icon("res://Template/Sprites/Icons/Gear/gear.svg")]
public partial class EnemyComponent : EntityComponent
{
    public Node2D Target { get; set; }

    public override void _Ready()
    {
        base._Ready();
        ServiceProvider.Services.Get<Level>().EnemyComponents.Add(this);
    }

    public override void SwitchState(NodeState newState, bool callExit = true)
    {
        base.SwitchState(newState, callExit);
    }
}
