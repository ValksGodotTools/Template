namespace GodotUtils;

using Godot;

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

    public static float RandRange(double min, double max) =>
        (float)GD.RandRange(min, max);

    public static int RandRange(int min, int max) => GD.RandRange(min, max);

    public static Vector2 RandDir(float dist = 1)
    {
        float theta = RandAngle();
        return new Vector2(Mathf.Cos(theta) * dist, Mathf.Sin(theta) * dist);
    }

    public static float RandAngle() => RandRange(0, Mathf.Pi * 2);

    /// <summary>
    /// <para>Returns the sum of the first n natural numbers</para>
    /// <para>For example if n = 4 then this would return 0 + 1 + 2 + 3</para>
    /// </summary>
    public static int SumNaturalNumbers(int n) => (n * (n - 1)) / 2;

    public static uint UIntPow(this uint x, uint pow)
    {
        uint ret = 1;
        while (pow != 0)
        {
            if ((pow & 1) == 1)
                ret *= x;
            x *= x;
            pow >>= 1;
        }
        return ret;
    }
}
