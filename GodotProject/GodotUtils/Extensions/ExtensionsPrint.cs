namespace GodotUtils;

using Godot;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

public static class ExtensionsPrint
{
    /// <summary>
    /// Prints a collection in a readable format
    /// </summary>
    public static string Print<T>(this IEnumerable<T> value, bool newLine = true) =>
        value != null ? string.Join(newLine ? "\n" : ", ", value) : null;

    /// <summary>
    /// Prints the entire object in a readable format (supports Godot properties)
    /// If you should ever run into a problem, see the IgnorePropsResolver class to ignore more
    /// properties.
    /// </summary>
    public static string PrintFull(this object v) =>
        JsonConvert.SerializeObject(v, Formatting.Indented, new JsonSerializerSettings
        {
            ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
            ContractResolver = new IgnorePropsResolver() // ignore all Godot props
        });

    /// <summary>
    /// Similar to the PrintFull() function except the contents are printed with GD.Print()
    /// </summary>
    public static void GDPrintFull(this object v) =>
        GD.Print(v.PrintFull());

    /// <summary>
    /// Used when doing JsonConvert.SerializeObject to ignore Godot properties
    /// as these are massive.
    /// </summary>
    class IgnorePropsResolver : DefaultContractResolver
    {
        protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization)
        {
            JsonProperty prop =
                base.CreateProperty(member, memberSerialization);

            // Ignored properties (prevents crashes)
            Type[] ignoredProps = new Type[]
            {
                typeof(GodotObject),
                typeof(Node),
                typeof(NodePath),
                typeof(ENet.Packet)
            };

            foreach (Type ignoredProp in ignoredProps)
            {
                if (ignoredProp.GetProperties().Contains(member))
                    prop.Ignored = true;

                if (prop.PropertyType == ignoredProp || prop.PropertyType.IsSubclassOf(ignoredProp))
                    prop.Ignored = true;
            }

            return prop;
        }
    }
}
