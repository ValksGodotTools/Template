using Godot;
using GodotUtils;
using System;
using System.Collections.Generic;

namespace Template.TopDown2D;

[GlobalClass]
public partial class SlideState : NodeState
{
    // 4  - Main
    // 8  - Main + Diagonal Directions
    // 16 - Looks the most natural
    private const int NUM_RAYCASTS = 16;
    private const int RAYCAST_LENGTH = 300;

    protected override void Enter()
    {
        Vector2 target = Entity.Position + GetRandomDirection() * RAYCAST_LENGTH;
        Vector2 force = (target - Entity.Position);

        Entity.ApplyCentralImpulse(force);

        new GTween(Sprite)
            .SetAnimatingProp(Node2D.PropertyName.Scale)
            .AnimateProp(Sprite.Scale * new Vector2(1.3f, 0.5f), 0.2).EaseOut()
            .AnimateProp(Sprite.Scale * new Vector2(1, 1), 0.3).EaseOut()
            .Callback(() => SwitchState(NextState));
    }

    /// <summary>
    /// Finds a random direction vector that does not collide with any obstacles. This method uses a series of raycasts to find a non-colliding direction. It starts at a random raycast angle and checks each direction in a circular pattern. If a non-colliding direction is found, it is returned. If no non-colliding direction is found after checking all possible directions, a completely random direction is returned.
    /// </summary>
    /// <returns>A <see cref="Vector2"/> representing a random direction that does not collide with any obstacles.</returns>
    /// <param name="NUM_RAYCASTS">The number of raycasts to perform in a full circle. This parameter should be defined elsewhere in the code.</param>
    /// <param name="RAYCAST_LENGTH">The length of each raycast. This parameter should be defined elsewhere in the code.</param>
    private Vector2 GetRandomDirection()
    {
        Vector2 direction = Vector2.Zero;
        List<RayCast2D> raycasts = [];
        bool foundNonCollidingDirection = false;

        // Start at a random raycast angle
        int startIndex = GD.RandRange(0, NUM_RAYCASTS);

        for (int i = 0; i < NUM_RAYCASTS; i++)
        {
            // The current raycast we are on
            int currentIndex = (startIndex + i) % NUM_RAYCASTS;

            float angle = currentIndex * (Mathf.Pi * 2) / NUM_RAYCASTS;
            direction = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));

            RayCast2D raycast = new();
            raycast.TargetPosition = direction * RAYCAST_LENGTH;
            raycasts.Add(raycast);
            AddChild(raycast);

            // Fully prepare the raycast before checking its collision
            raycast.ForceRaycastUpdate();

            if (!raycast.IsColliding())
            {
                foundNonCollidingDirection = true;
                break;
            }
        }

        raycasts.ForEach(raycast => raycast.QueueFree());

        if (!foundNonCollidingDirection)
        {
            direction = GMath.RandDir();
        }

        return direction;
    }
}
