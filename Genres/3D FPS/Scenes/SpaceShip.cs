using Godot;
using System;

namespace Template.FPS3D;

[SceneTree]
public partial class SpaceShip : Node
{
    public override void _Ready()
    {
        var t = _.Area3D;
    
        
        _.Area3D.BodyEntered += body =>
        {
            if (body is Player player)
            {
                player.QueueFree();
                GetNode<Camera3D>("Camera3D").MakeCurrent();
            }
        };
    }

    public override void _PhysicsProcess(double delta)
    {
        
    }
}
