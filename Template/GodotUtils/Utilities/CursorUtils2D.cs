using Godot;
using System.Collections.Generic;

namespace GodotUtils;

/// <summary>
/// Utility class for handling cursor-related operations in a 2D Godot scene.
/// </summary>
public static class CursorUtils2D
{
    /// <summary>
    /// Retrieves the <see cref="Area2D"/> node under the cursor's current position.
    /// </summary>
    /// <param name="node">The <see cref="Node2D"/> node from which to start the query.</param>
    /// <returns>The <see cref="Area2D"/> node under the cursor, or null if none is found.</returns>
    public static Area2D GetAreaUnderCursor(Node2D node)
    {
        return (Area2D)GetPhysicsNodeAtPosition(node, node.GetGlobalMousePosition(),
            true, false, false);
    }

    /// <summary>
    /// Retrieves the <see cref="PhysicsBody2D"/> node under the cursor's current position.
    /// </summary>
    /// <param name="node">The <see cref="Node2D"/> node from which to start the query.</param>
    /// <returns>The <see cref="PhysicsBody2D"/> node under the cursor, or null if none is found.</returns>
    public static PhysicsBody2D GetBodyUnderCursor(Node2D node)
    {
        return (PhysicsBody2D)GetPhysicsNodeAtPosition(node, node.GetGlobalMousePosition(),
            false, true, false);
    }

    /// <summary>
    /// Retrieves the <see cref="Area2D"/> node under the specified <see cref="Node2D"/> node's global position.
    /// </summary>
    /// <param name="node">The <see cref="Node2D"/> node from which to start the query.</param>
    /// <returns>The <see cref="Area2D"/> node under the specified position, or null if none is found.</returns>
    public static Area2D GetAreaUnder(Node2D node)
    {
        return (Area2D)GetPhysicsNodeAtPosition(node, node.GlobalPosition,
            true, false, true);
    }

    /// <summary>
    /// Retrieves the <see cref="PhysicsBody2D"/> node under the specified <see cref="Node2D"/> node's global position.
    /// </summary>
    /// <param name="node">The <see cref="Node2D"/> node from which to start the query.</param>
    /// <returns>The <see cref="PhysicsBody2D"/> node under the specified position, or null if none is found.</returns>
    public static PhysicsBody2D GetBodyUnder(Node2D node)
    {
        return (PhysicsBody2D)GetPhysicsNodeAtPosition(node, node.GlobalPosition,
            false, true, true);
    }

    /// <summary>
    /// Retrieves a physics node (either <see cref="Area2D"/> or <see cref="PhysicsBody2D"/>) at the specified position.
    /// </summary>
    /// <param name="node">The <see cref="Node2D"/> node from which to start the query.</param>
    /// <param name="position">The position in the world to query.</param>
    /// <param name="collideWithAreas">Whether to collide with <see cref="Area2D"/> nodes.</param>
    /// <param name="collideWithBodies">Whether to collide with <see cref="PhysicsBody2D"/> nodes.</param>
    /// <param name="excludeSelf">Whether to exclude the node itself and its children from the query.</param>
    /// <returns>The physics node at the specified position, or null if none is found.</returns>
    private static Node GetPhysicsNodeAtPosition(Node2D node, Vector2 position, bool collideWithAreas, bool collideWithBodies, bool excludeSelf = false)
    {
        // Create a shape query parameters object
        PhysicsPointQueryParameters2D queryParams = new();
        queryParams.Position = position;
        queryParams.CollideWithAreas = collideWithAreas;
        queryParams.CollideWithBodies = collideWithBodies;

        if (excludeSelf)
        {
            List<Rid> rids = [];

            foreach (Node child in node.GetChildren<Node>())
            {
                if (child is CollisionObject2D collision)
                {
                    rids.Add(collision.GetRid());
                }
            }

            queryParams.Exclude = new Godot.Collections.Array<Rid>(rids);
        }

        // Perform the query
        PhysicsDirectSpaceState2D spaceState =
            PhysicsServer2D.SpaceGetDirectState(node.GetWorld2D().GetSpace());

        Godot.Collections.Array<Godot.Collections.Dictionary> results =
            spaceState.IntersectPoint(queryParams, 1);

        foreach (Godot.Collections.Dictionary result in results)
        {
            if (result != null && result.ContainsKey("collider"))
            {
                return result["collider"].As<Node>();
            }
        }

        return null;
    }
}
