using Godot;
using GodotUtils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Template.DragAndDrop;

public partial class DragTestScene : Node
{
    public override void _Ready()
    {
        Type[] types = Assembly.GetExecutingAssembly().GetTypes();

        Dictionary<Type, DraggableAttribute> cache = [];

        foreach (Type type in types)
        {
            DraggableAttribute draggableAttribute = (DraggableAttribute)type.GetCustomAttribute(typeof(DraggableAttribute));

            if (draggableAttribute != null)
            {
                cache.Add(type, draggableAttribute);
            }
        }

        IEnumerable<Node> nodes = this.GetChildren<Node>().Where(n => n is Node2D or Control);

        foreach (Node node in nodes)
        {
            foreach (KeyValuePair<Type, DraggableAttribute> kvp in cache)
            {
                if (kvp.Key.IsAssignableTo(node.GetType()))
                {
                    GD.Print(node.Name);
                    break;
                }
            }
        }
    }
}
