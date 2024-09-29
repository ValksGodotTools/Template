using Godot;

namespace GodotUtils;

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
        {
            num |= 1 << layer - 1;
        }

        return num;
    }

    public static float RandRange(double min, double max)
    {
        return (float)GD.RandRange(min, max);
    }

    public static int RandRange(int min, int max)
    {
        return GD.RandRange(min, max);
    }

    public static Vector2 RandDir()
    {
        float theta = RandAngle();
        return new Vector2(Mathf.Cos(theta), Mathf.Sin(theta));
    }

    public static float RandAngle()
    {
        return RandRange(0, Mathf.Pi * 2);
    }
}
