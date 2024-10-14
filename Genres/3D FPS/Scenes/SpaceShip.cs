using Godot;
using System;

namespace Template.FPS3D;

public partial class SpaceShip : Node
{
    public override void _Ready()
    {
        GetNode<Area3D>("Area3D").BodyEntered += body =>
        {
            if (body is Player player)
            {
                // The fun part here
            }
        };
    }
}
