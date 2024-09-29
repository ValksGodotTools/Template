using Godot;

namespace GodotUtils;

public static class CollisionObject2DExtensions
{
    /// <summary>
    /// Set the <paramref name="layers"/> for CollisionLayer and CollisionMask
    /// </summary>
    public static void SetCollisionLayerAndMask(this CollisionObject2D collisionObject, params int[] layers)
    {
        collisionObject.CollisionLayer = (uint)GMath.GetLayerValues(layers);
        collisionObject.CollisionMask = (uint)GMath.GetLayerValues(layers);
    }
}

