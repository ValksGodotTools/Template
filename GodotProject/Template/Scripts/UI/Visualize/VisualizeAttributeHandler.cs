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
                List<PropertyInfo> properties = GetMembersWithAttribute(type, type.GetProperties);
                List<FieldInfo> fields = GetMembersWithAttribute(type, type.GetFields);
                List<MethodInfo> methods = GetMembersWithAttribute(type, type.GetMethods);

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

    private static List<T> GetMembersWithAttribute<T>(Type type, Func<BindingFlags, T[]> getMembers) where T : MemberInfo
    {
        return getMembers(Flags)
            .Where(member => member.GetCustomAttributes(typeof(VisualizeAttribute), false).Any())
            .ToList();
    }
}
