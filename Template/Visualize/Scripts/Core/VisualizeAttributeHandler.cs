using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Template;

/// <summary>
/// Handles the VisualizeAttribute
/// </summary>
public static class VisualizeAttributeHandler
{
    private static readonly BindingFlags _flags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static;

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

        if (properties.Count != 0 || fields.Count != 0 || methods.Count != 0 || (attribute != null && attribute.VisualizeMembers != null))
        {
            return new VisualNode(specificNode, initialPosition, visualizeMembers, properties, fields, methods);
        }

        return null;
    }

    private static List<T> GetVisualMembers<T>(Func<BindingFlags, T[]> getMembers) where T : MemberInfo
    {
        return getMembers(_flags)
            .Where(member => member.GetCustomAttributes(typeof(VisualizeAttribute), false).Length != 0)
            .ToList();
    }
}
