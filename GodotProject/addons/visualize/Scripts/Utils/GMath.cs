namespace Template;

public static class GMath
{
    /// <summary>
    /// Godot has nodes with properties like LightMask, CollisionLayer
    /// and MaskLayer. All these properties are integers. Simply setting
    /// the property to a number like 5 won't enable the 5th layer. The
    /// number needs to be in binary and that is what this function is for.
    /// 
    /// <para>For example to enable the 1st, 4th and 5th layers of a players
    /// collision layer you would do the following.</para>
    /// 
    /// <code>player.CollisionLayer = GU.GetLayerValues(1, 4, 5)</code>
    /// </summary>
    public static int GetLayerValues(params int[] layers)
    {
        int num = 0;

        foreach (int layer in layers)
            num |= 1 << layer - 1;

        return num;
    }
}
