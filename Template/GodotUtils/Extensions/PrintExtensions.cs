using Godot;
using Newtonsoft.Json.Serialization;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System;

namespace GodotUtils;

/// <summary>
/// Provides extension methods for printing objects and collections in a formatted manner.
/// </summary>
public static class PrintExtensions
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
    /// Converts the object to a formatted JSON string and prints it.
    /// </summary>
    /// <param name="v">The object to print.</param>
    public static void PrintFormatted(this object v)
    {
        GD.Print(v.ToFormattedString());
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
        {
            return null;
        }

        if (newLine)
        {
            return "[\n    " + string.Join(",\n    ", value) + "\n]";
        }
        else
        {
            return "[" + string.Join(", ", value) + "]";
        }
    }

    /// <summary>
    /// Converts the object to a formatted JSON string.
    /// </summary>
    /// <param name="v">The object to convert.</param>
    /// <returns>A formatted JSON string representation of the object.</returns>
    public static string ToFormattedString(this object v)
    {
        return JsonConvert.SerializeObject(v, Formatting.Indented, new JsonSerializerSettings
        {
            ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
            ContractResolver = new IgnorePropsResolver() // ignore all Godot props
        });
    }

    /// <summary>
    /// A custom contract resolver used to ignore certain Godot properties during JSON serialization.
    /// </summary>
    private class IgnorePropsResolver : DefaultContractResolver
    {
        /// <summary>
        /// Creates a JsonProperty for the given MemberInfo.
        /// </summary>
        /// <param name="member">The MemberInfo to create a JsonProperty for.</param>
        /// <param name="memberSerialization">The MemberSerialization mode for the member.</param>
        /// <returns>A JsonProperty for the given MemberInfo.</returns>
        protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization)
        {
            JsonProperty prop =
                base.CreateProperty(member, memberSerialization);

            // Ignored properties (prevents crashes)
            Type[] ignoredProps =
            [
                typeof(GodotObject),
                typeof(Node),
                typeof(NodePath),
                typeof(ENet.Packet)
            ];

            foreach (Type ignoredProp in ignoredProps)
            {
                if (ignoredProp.GetProperties().Contains(member))
                {
                    prop.Ignored = true;
                }

                if (prop.PropertyType == ignoredProp || prop.PropertyType.IsSubclassOf(ignoredProp))
                {
                    prop.Ignored = true;
                }
            }

            return prop;
        }
    }
}
