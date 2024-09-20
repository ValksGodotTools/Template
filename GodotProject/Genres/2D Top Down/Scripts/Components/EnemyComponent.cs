using Godot;

namespace Template.TopDown2D;

/// <summary>
/// This script assumes all enemies are of type Node2D and have animated sprite nodes.
/// </summary>
[GlobalClass, Icon("res://Template/Sprites/Icons/Gear/gear.svg")]
public partial class EnemyComponent : EntityComponent
{
    public Player Player { get; set; }

    public override void _Ready()
    {
        base._Ready();
        ServiceProvider.Services.Get<Level>().EnemyComponents.Add(this);
    }

    public override void SwitchState(NodeState newState)
    {
        // Enemies will always stay in the idle state when no player is present
        if (!IsInstanceValid(Player))
        {
            // Do not call SwitchState() to prevent infinite loop
            _curState.Exit();
            _curState = IdleState.State;
            _curState.Enter();

            return;
        }

        base.SwitchState(newState);
    }
}
