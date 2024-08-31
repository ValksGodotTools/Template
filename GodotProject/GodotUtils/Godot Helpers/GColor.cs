namespace GodotUtils;

using Godot;

public class GColor
{
    /// <summary>
    /// <para>Create a color from HSV (Hue, Saturation, Value)</para>
    /// <para>'hue' - values range from 0 to 359</para>
    /// <para>'saturation' - values range from 0 to 100</para>
    /// <para>'value' - values range from 0 to 100</para>
    /// <para>'alpha' - values range from 0 to 255</para>
    /// </summary>
    public static Color FromHSV(int hue, int saturation = 100, int value = 100, int alpha = 255)
    {
        return Color.FromHsv(hue / 359f, saturation / 100f, value / 100f, alpha / 255f);
    }

    /// <summary>
    /// Generate a random color
    /// </summary>
    public static Color Random(int alpha = 255)
    {
        float r = GMath.RandRange(0.0, 1.0);
        float g = GMath.RandRange(0.0, 1.0);
        float b = GMath.RandRange(0.0, 1.0);

        return new Color(r, g, b, alpha / 255f);
    }
}
