namespace GodotUtils;

using Godot;

public static class ExtensionsCollisionObject2D
{
    /// <summary>
    /// Set the specified layer and mask values to true. Everything else will 
    /// be set to be false.
    /// </summary>
    public static void SetCollisionMaskLayer(this CollisionObject2D node, params int[] values)
    {
        // Reset all layer and mask values to 0
        node.CollisionLayer = 0;
        node.CollisionMask = 0;

        foreach (int value in values)
        {
            node.SetCollisionLayerValue(value, true);
            node.SetCollisionMaskValue(value, true);
        }
    }

    /// <summary>
    /// Set the specified mask values to true. Everything else will 
    /// be set to be false.
    /// </summary>
    public static void SetCollisionMask(this CharacterBody2D node, params int[] values)
    {
        // Reset all mask values to 0
        node.CollisionMask = 0;

        foreach (int value in values)
        {
            node.SetCollisionMaskValue(value, true);
        }
    }

    /// <summary>
    /// Set the specified layer values to true. Everything else will 
    /// be set to be false.
    /// </summary>
    public static void SetCollisionLayer(this CharacterBody2D node, params int[] values)
    {
        // Reset all layer values to 0
        node.CollisionLayer = 0;

        foreach (int value in values)
        {
            node.SetCollisionLayerValue(value, true);
        }
    }
}
