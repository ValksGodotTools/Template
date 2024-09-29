using Godot;
using System;

namespace GodotUtils;

public static class MathExtensions
{
    public static Color Lerp(this Color color1, Color color2, float t)
    {
        return color1 * (1 - t) + color2 * t;
    }

    public static float Remap(this float value, float from1, float to1, float from2, float to2)
    {
        return (value - from1) / (to1 - from1) * (to2 - from2) + from2;
    }

    public static void LerpRotationToTarget(this Sprite2D sprite, Vector2 target, float t = 0.1f)
    {
        sprite.Rotation = Mathf.LerpAngle(sprite.Rotation, (target - sprite.GlobalPosition).Angle(), t);
    }

    public static float ToRadians(this float degrees)
    {
        return degrees * (Mathf.Pi / 180);
    }

    public static float ToDegrees(this float radians)
    {
        return radians * (180 / Mathf.Pi);
    }

    public static int Clamp(this int v, int min, int max)
    {
        return Mathf.Clamp(v, min, max);
    }

    public static float Clamp(this float v, float min, float max)
    {
        return Mathf.Clamp(v, min, max);
    }

    public static float Lerp(this float a, float b, float t)
    {
        return Mathf.Lerp(a, b, t);
    }

    /// <summary>
    /// Pulses a value from 0 to 1 to 0 to 1 over time
    /// </summary>
    public static float Pulse(this float time, float frequency)
    {
        return 0.5f * (1 + Mathf.Sin(2 * Mathf.Pi * frequency * time));
    }

    /// <summary>
    /// Counts the number of digits in 'num'. For example 123 has 3 digits.
    /// </summary>
    public static int CountDigits(this int num)
    {
        return (int)Math.Floor(Math.Log10(num) + 1);
    }

    /// <summary>
    /// Counts the number of digits in 'num'. For example 123 has 3 digits.
    /// </summary>
    public static ushort CountDigits(this ushort num)
    {
        return (ushort)Math.Floor(Math.Log10(num) + 1);
    }
}

