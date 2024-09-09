using System;
using System.Collections.Generic;
using System.Linq;
using Godot;

namespace Visualize.Utils;

public static class Extensions
{
    /// <summary>
    /// Set the <paramref name="layers"/> for CollisionLayer and CollisionMask
    /// </summary>
    public static void SetCollisionLayerAndMask(this CollisionObject2D collisionObject, params int[] layers)
    {
        collisionObject.CollisionLayer = (uint)GMath.GetLayerValues(layers);
        collisionObject.CollisionMask = (uint)GMath.GetLayerValues(layers);
    }

    public static void SetUnshaded(this CanvasItem canvasItem)
    {
        canvasItem.Material = new CanvasItemMaterial
        {
            LightMode = CanvasItemMaterial.LightModeEnum.Unshaded
        };
    }

    /// <summary>
    /// Recursively searches for all nodes of <paramref name="type"/>
    /// </summary>
    public static List<Node> GetNodes(this Node node, Type type)
    {
        List<Node> nodes = [];
        RecursiveTypeMatchSearch(node, type, nodes);
        return nodes;
    }

    private static void RecursiveTypeMatchSearch(Node node, Type type, List<Node> nodes)
    {
        if (node.GetType() == type)
        {
            nodes.Add(node);
        }

        foreach (Node child in node.GetChildren())
        {
            RecursiveTypeMatchSearch(child, type, nodes);
        }
    }

    /// <summary>
    /// Recursively retrieves all nodes of type <typeparamref name="T"/> from <paramref name="node"/>
    /// </summary>
    public static List<T> GetChildren<T>(this Node node) where T : Node
    {
        List<T> children = new();
        FindChildrenOfType<T>(node, children);
        return children;
    }

    private static void FindChildrenOfType<T>(Node node, List<T> children) where T : Node
    {
        foreach (Node child in node.GetChildren())
        {
            if (child is T typedChild)
            {
                children.Add(typedChild);
            }

            FindChildrenOfType<T>(child, children);
        }
    }

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
    /// This will transform for example "helloWorld" to "hello World"
    /// </summary>
    public static string AddSpaceBeforeEachCapital(this string v) =>
        string.Concat(v.Select(x => char.IsUpper(x) ? " " + x : x.ToString())).TrimStart(' ');
}
