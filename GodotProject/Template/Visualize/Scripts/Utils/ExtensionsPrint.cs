using Godot;
using System.Collections.Generic;

namespace Template;

/// <summary>
/// Provides extension methods for printing objects and collections in a formatted manner.
/// </summary>
public static class ExtensionsPrint
{
    /// <summary>
    /// Prints the elements of an enumerable collection in a formatted string.
    /// </summary>
    /// <typeparam name="T">The type of elements in the enumerable collection.</typeparam>
    /// <param name="value">The enumerable collection to print.</param>
    /// <param name="newLine">If true, each element will be printed on a new line; otherwise, elements will be printed on the same line separated by commas.</param>
    public static void PrintFormatted<T>(this IEnumerable<T> value, bool newLine = true)
    {
        GD.Print(value.ToFormattedString(newLine));
    }

    /// <summary>
    /// Converts the elements of an enumerable collection to a formatted string.
    /// </summary>
    /// <typeparam name="T">The type of elements in the enumerable collection.</typeparam>
    /// <param name="value">The enumerable collection to convert.</param>
    /// <param name="newLine">If true, each element will be printed on a new line; otherwise, elements will be printed on the same line separated by commas.</param>
    /// <returns>A formatted string representation of the enumerable collection.</returns>
    public static string ToFormattedString<T>(this IEnumerable<T> value, bool newLine = true)
    {
        if (value == null)
            return null;

        if (newLine)
        {
            return "[\n    " + string.Join(",\n    ", value) + "\n]";
        }
        else
        {
            return "[" + string.Join(", ", value) + "]";
        }
    }
}
