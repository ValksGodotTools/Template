using Godot;

namespace GodotUtils;

public static class RayCast2DExtensions
{
    /// <summary>
    /// <para>
    /// Get the tile from a tilemap that a raycast is colliding with.
    /// Use tileData.Equals(default(Variant)) to check if no tile data exists
    /// here.
    /// </para>
    /// 
    /// <para>
    /// Useful if trying to detect what tile the player is standing on
    /// </para>
    /// 
    /// <para>
    /// To get the tile the player is currently in see TileMap.GetTileData(...)
    /// </para>
    /// </summary>
    public static Variant GetTileData(this RayCast2D raycast, string layerName)
    {
        if (!raycast.IsColliding() || raycast.GetCollider() is not TileMapLayer tileMap)
        {
            return default;
        }

        Vector2 collisionPos = raycast.GetCollisionPoint();
        Vector2I tilePos = tileMap.LocalToMap(tileMap.ToLocal(collisionPos));

        TileData tileData = tileMap.GetCellTileData(tilePos);

        if (tileData == null)
        {
            return default;
        }

        return tileData.GetCustomData(layerName);
    }

    /// <summary>
    /// Set the provided mask values to true. Everything else will be set to be false.
    /// </summary>
    public static void SetCollisionMask(this RayCast2D node, params int[] values)
    {
        // Reset all mask values to 0
        node.CollisionMask = 0;

        foreach (int value in values)
        {
            node.SetCollisionMaskValue(value, true);
        }
    }

    /// <summary>
    /// A convience function to tell the raycast to exlude all parents that
    /// are of type CollisionObject2D (for example a ground raycast should
    /// only check for the ground, not the player itself)
    /// </summary>
    public static void ExcludeRaycastParents(this RayCast2D raycast, Node parent)
    {
        if (parent != null)
        {
            if (parent is CollisionObject2D collision)
            {
                raycast.AddException(collision);
            }

            ExcludeRaycastParents(raycast, parent.GetParentOrNull<Node>());
        }
    }

    /// <summary>
    /// Checks if any raycasts in a collection is colliding
    /// </summary>
    /// <param name="raycasts">Collection of raycasts to check</param>
    /// <returns>True if any ray cast is colliding, else false</returns>
    public static bool IsAnyRayCastColliding(this RayCast2D[] raycasts)
    {
        foreach (RayCast2D raycast in raycasts)
        {
            if (raycast.IsColliding())
            {
                return true;
            }
        }

        return false;
    }

    /// <summary>
    /// Returns the first raycasts in a collection which is colliding
    /// </summary>
    /// <param name="raycasts">Collection of raycasts to check</param>
    /// <returns>Raycast which is colliding, else default</returns>
    public static RayCast2D GetAnyRayCastCollider(this RayCast2D[] raycasts)
    {
        foreach (RayCast2D raycast in raycasts)
        {
            if (raycast.IsColliding())
            {
                return raycast;
            }
        }

        return default;
    }
}

