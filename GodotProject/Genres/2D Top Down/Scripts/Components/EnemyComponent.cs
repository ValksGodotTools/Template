using Godot;

namespace Template.TopDown2D;

/// <summary>
/// This script assumes all enemies are of type Node2D and have animated sprite nodes.
/// </summary>
[GlobalClass, Icon("res://Template/Sprites/Icons/Gear/gear.svg")]
[Visualize(nameof(_curState))]
public partial class EnemyComponent : EntityComponent
{
    public Player Player { get; set; }
}
