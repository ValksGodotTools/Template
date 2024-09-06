using Godot;
using GodotUtils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Template;

public static class VisualizeAttributeHandler
{
    public static List<DebugVisualNode> RetrieveData(Node parent)
    {
        List<DebugVisualNode> debugVisualNodes = [];

        Type[] types = Assembly.GetExecutingAssembly().GetTypes();

        IEnumerable<Type> visualTypes = types.Where(x => x.GetCustomAttributes(typeof(VisualizeAttribute), false).Any());

        BindingFlags flags =
            BindingFlags.Public |
            BindingFlags.NonPublic |
            BindingFlags.Instance |
            BindingFlags.Static;

        foreach (Type type in types)
        {
            Vector2 initialPosition = Vector2.Zero;
            VisualizeAttribute attribute = (VisualizeAttribute)type.GetCustomAttribute(typeof(VisualizeAttribute), false);

            if (attribute != null)
            {
                initialPosition = attribute.InitialPosition;
            }

            List<Node> nodes = parent.GetNodes(type);

            foreach (Node node in nodes)
            {
                List<PropertyInfo> properties = [];
                List<FieldInfo> fields = [];
                List<MethodInfo> methods = [];

                foreach (PropertyInfo property in type.GetProperties(flags))
                {
                    if (property.GetCustomAttributes(typeof(VisualizeAttribute), false).Any())
                    {
                        properties.Add(property);
                    }
                }

                foreach (FieldInfo field in type.GetFields(flags))
                {
                    if (field.GetCustomAttributes(typeof(VisualizeAttribute), false).Any())
                    {
                        fields.Add(field);
                    }
                }

                foreach (MethodInfo method in type.GetMethods(flags))
                {
                    if (method.GetCustomAttributes(typeof(VisualizeAttribute), false).Any())
                    {
                        methods.Add(method);
                    }
                }

                if (properties.Any() || fields.Any() || methods.Any())
                {
                    debugVisualNodes.Add(new DebugVisualNode(node, initialPosition, properties, fields, methods));
                }
            }
        }

        return debugVisualNodes;
    }
}
