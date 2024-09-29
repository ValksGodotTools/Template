using System;
using System.Collections.Generic;

namespace GodotUtils;

public static class TypeRangeConstraints
{
    public static (object Min, object Max) GetRange(Type type)
    {
        if (Constraints.TryGetValue(type, out (object Min, object Max) range))
        {
            return range;
        }

        throw new ArgumentException($"Type {type.FullName} is not supported.");
    }

    private static Dictionary<Type, (object Min, object Max)> Constraints { get; } = new()
    {
        { typeof(sbyte), (sbyte.MinValue, sbyte.MaxValue) },
        { typeof(short), (short.MinValue, short.MaxValue) },
        { typeof(int), (int.MinValue, int.MaxValue) },
        { typeof(long), (long.MinValue, long.MaxValue) },
        { typeof(float), (float.MinValue, float.MaxValue) },
        { typeof(double), (double.MinValue, double.MaxValue) },
        { typeof(decimal), (decimal.MinValue, decimal.MaxValue) },
        { typeof(byte), (byte.MinValue, byte.MaxValue) },
        { typeof(ushort), (ushort.MinValue, ushort.MaxValue) },
        { typeof(uint), (uint.MinValue, uint.MaxValue) },
        { typeof(ulong), (ulong.MinValue, ulong.MaxValue) }
    };
}
