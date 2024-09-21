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
        Dictionary<Type, DraggableAttribute> cache = CacheDraggableAttributes();
        IEnumerable<Node> nodes = GetDraggableNodes(this);

        foreach (Node node in nodes)
        {
            foreach (KeyValuePair<Type, DraggableAttribute> kvp in cache)
            {
                if (kvp.Key.IsAssignableTo(node.GetType()))
                {
                    MakeNodeDraggable(node);
                    break;
                }
            }
        }
    }

    private static void MakeNodeDraggable(Node node)
    {
        Vector2 size = GetNodeSize(node);

        Area2D area = new();
        CollisionShape2D collision = new()
        {
            Shape = new RectangleShape2D
            {
                Size = size
            }
        };

        area.AddChild(collision);
        node.AddChild(area);
    }

    private Dictionary<Type, DraggableAttribute> CacheDraggableAttributes()
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

        return cache;
    }

    private static IEnumerable<Node> GetDraggableNodes(Node root)
    {
        return root.GetChildren<Node>().Where(n => n is Node2D or Control);
    }

    private static Vector2 GetNodeSize(Node node)
    {
        Vector2 size = Vector2.Zero;

        if (node is Sprite2D sprite)
        {
            size = sprite.GetSize();
        }
        else if (node is Control control)
        {
            size = control.Size;
        }

        return size;
    }
}
