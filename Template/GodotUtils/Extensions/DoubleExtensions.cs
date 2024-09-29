namespace GodotUtils;

public static class DoubleExtensions
{
    /// <summary>
    /// Checks to see if <paramref name="value"/> is an integer.
    /// 
    /// <para>
    /// Since double.IsInteger(double d) is only in .NET 7, this extension 
    /// will act as a helper for .NET 6 users.
    /// </para>
    /// </summary>
    public static bool IsInteger(this double value)
    {
        return (value % 1) == 0;
    }
}
