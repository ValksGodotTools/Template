using System.Collections.Generic;
using System;

namespace GodotUtils;

public static class TypeExtensions
{
    /// <summary>
    /// Returns true if the <paramref name="type"/> is a numeric type
    /// <para>For .NET 6.0 and later, consider using the INumber interface for a more generic approach.</para>
    /// </summary>
    public static bool IsNumericType(this Type @type)
    {
        HashSet<Type> numericTypes =
        [
            typeof(int),
            typeof(float),
            typeof(double),
            typeof(long),
            typeof(short),
            typeof(ushort),
            typeof(uint),
            typeof(ulong),
            typeof(decimal),
            typeof(byte),
            typeof(sbyte)
        ];

        return numericTypes.Contains(type);
    }

    /// <summary>
    /// Returns true if the <paramref name="type"/> is a whole number.
    /// </summary>
    public static bool IsWholeNumber(this Type type)
    {
        return
            type == typeof(int) ||
            type == typeof(long) ||
            type == typeof(short) ||
            type == typeof(byte) ||
            type == typeof(sbyte) ||
            type == typeof(uint) ||
            type == typeof(ulong) ||
            type == typeof(ushort);
    }
}
