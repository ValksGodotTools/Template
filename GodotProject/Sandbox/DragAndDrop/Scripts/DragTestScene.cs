using Godot;
using GodotUtils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Template.DragAndDrop;

public interface IDraggableNode
{
    public Vector2 Position { get; set; }
}

public partial class DragTestScene : Node2D
{
    private Node2D _selectedNode;
    private Node2D _currentlyDraggedNode;

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

    public override void _Input(InputEvent @event)
    {
        if (@event is InputEventMouseButton btn)
        {
            if (btn.IsLeftClickPressed())
            {
                if (_selectedNode != null)
                {
                    // Reparent to viewport root
                    _selectedNode.Reparent(GetTree().Root);

                    _currentlyDraggedNode = _selectedNode;
                }
            }
        }
    }

    public override void _PhysicsProcess(double delta)
    {
        if (_currentlyDraggedNode != null)
        {
            _currentlyDraggedNode.GlobalPosition = GetGlobalMousePosition();
        }
    }

    private void MakeNodeDraggable(Node node)
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

        area.MouseEntered += () =>
        {
            _selectedNode = node as Node2D;
        };
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
