using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Visualize.Utils;

namespace Visualize.Core;

public static class VisualizeAttributeHandler
{
    private static readonly BindingFlags Flags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static;

    public static VisualNode RetrieveData(Node specificNode)
    {
        Type type = specificNode.GetType();

        VisualizeAttribute attribute = (VisualizeAttribute)type.GetCustomAttribute(typeof(VisualizeAttribute), false);

        Vector2 initialPosition = Vector2.Zero;
        string[] visualizeMembers = null;

        if (attribute != null)
        {
            initialPosition = attribute.InitialPosition;
            visualizeMembers = attribute.VisualizeMembers;
        }

        List<PropertyInfo> properties = GetVisualMembers(type.GetProperties);
        List<FieldInfo> fields = GetVisualMembers(type.GetFields);
        List<MethodInfo> methods = GetVisualMembers(type.GetMethods);

        if (properties.Any() || fields.Any() || methods.Any() || (attribute != null && attribute.VisualizeMembers != null))
        {
            return new VisualNode(specificNode, initialPosition, visualizeMembers, properties, fields, methods);
        }

        return null;
    }

    private static List<T> GetVisualMembers<T>(Func<BindingFlags, T[]> getMembers) where T : MemberInfo
    {
        return getMembers(Flags)
            .Where(member => member.GetCustomAttributes(typeof(VisualizeAttribute), false).Any())
            .ToList();
    }
}
