using Godot;
using GodotUtils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Template;

public static class VisualizeAttributeHandler
{
    private static readonly BindingFlags Flags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static;

    public static List<DebugVisualNode> RetrieveData(Node parent)
    {
        List<DebugVisualNode> debugVisualNodes = [];
        Type[] types = Assembly.GetExecutingAssembly().GetTypes();

        foreach (Type type in types)
        {
            Vector2 initialPosition = GetInitialPosition(type);
            List<Node> nodes = parent.GetNodes(type);

            foreach (Node node in nodes)
            {
                List<PropertyInfo> properties = GetVisualMembers(type.GetProperties);
                List<FieldInfo> fields = GetVisualMembers(type.GetFields);
                List<MethodInfo> methods = GetVisualMembers(type.GetMethods);

                if (properties.Any() || fields.Any() || methods.Any())
                {
                    debugVisualNodes.Add(new DebugVisualNode(node, initialPosition, properties, fields, methods));
                }
            }
        }

        return debugVisualNodes;
    }

    private static Vector2 GetInitialPosition(Type type)
    {
        VisualizeAttribute attribute = (VisualizeAttribute)type.GetCustomAttribute(typeof(VisualizeAttribute), false);
        
        return attribute?.InitialPosition ?? Vector2.Zero;
    }

    private static List<T> GetVisualMembers<T>(Func<BindingFlags, T[]> getMembers) where T : MemberInfo
    {
        return getMembers(Flags)
            .Where(member => member.GetCustomAttributes(typeof(VisualizeAttribute), false).Any())
            .ToList();
    }
}
