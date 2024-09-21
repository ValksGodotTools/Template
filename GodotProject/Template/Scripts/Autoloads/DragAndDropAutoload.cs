using Godot;
using GodotUtils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Template;

public partial class DragAndDropAutoload : Node2D
{
    private DraggableWrapper _selectedNode;
    private IDraggableNode _previousParent;
    private Vector2 _previousPosition;
    private Vector2 _dragControlOffset;
    private IDraggableNode _currentlyDraggedNode;

    public override void _Ready()
    {
        Dictionary<Type, DraggableAttribute> cache = CacheDraggableAttributes();
        IEnumerable<Node> nodes = GetDraggableNodes(GetTree().Root);

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

        GetTree().NodeAdded += OnNodeAdded;
    }

    public override void _Input(InputEvent @event)
    {
        if (@event is InputEventMouseButton btn)
        {
            if (btn.IsLeftClickPressed())
            {
                if (_selectedNode != null)
                {
                    _dragControlOffset = _selectedNode.DragControlOffset;
                    _previousParent = _selectedNode.GetParent<IDraggableNode>();
                    _previousPosition = _selectedNode.GlobalPosition;

                    // Reparent to viewport root
                    _selectedNode.Reparent(GetTree().Root as IDraggableNode);

                    _currentlyDraggedNode = _selectedNode;
                }
            }

            if (btn.IsLeftClickReleased() && _currentlyDraggedNode != null)
            {
                _currentlyDraggedNode = null;

                _selectedNode.Reparent(_previousParent);
                _selectedNode.GlobalPosition = _previousPosition;
                _selectedNode = null;
            }
        }
    }

    public override void _PhysicsProcess(double delta)
    {
        if (_currentlyDraggedNode != null)
        {
            _currentlyDraggedNode.GlobalPosition = GetGlobalMousePosition() - _dragControlOffset;
        }
    }

    private void OnNodeAdded(Node node)
    {
        DraggableAttribute attribute = node.GetType().GetCustomAttribute<DraggableAttribute>();

        if (attribute != null)
        {
            MakeNodeDraggable(node);
        }
    }

    private void MakeNodeDraggable(Node node)
    {
        Vector2 size = GetNodeSize(node);

        Area2D area = new();

        Vector2 dragControlOffset = Vector2.Zero;

        if (node is Control control)
        {
            dragControlOffset = size * 0.5f;

            area.Position += dragControlOffset;
            control.MouseFilter = Control.MouseFilterEnum.Ignore;
        }

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
            if (_currentlyDraggedNode == null)
            {
                _selectedNode = new DraggableWrapper(node, dragControlOffset);
            }
        };

        area.MouseExited += () =>
        {
            if (_currentlyDraggedNode == null)
            {
                _selectedNode = null;
            }
        };
    }

    private Dictionary<Type, DraggableAttribute> CacheDraggableAttributes()
    {
        Type[] types = Assembly.GetExecutingAssembly().GetTypes();
        Dictionary<Type, DraggableAttribute> cache = [];

        foreach (Type type in types)
        {
            DraggableAttribute attribute = type.GetCustomAttribute<DraggableAttribute>();

            if (attribute != null)
            {
                cache.Add(type, attribute);
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

// Needed to prevent needing the user to implement IDraggableNode on the node they want to drag
// User should only have to add [Draggable] attribute and everything else should be handled internally.
public class DraggableWrapper : IDraggableNode
{
    public Vector2 DragControlOffset { get; private set; }

    private Node2D _node2D;
    private Control _control;

    public DraggableWrapper(Node node, Vector2 dragControlOffset)
    {
        DragControlOffset = dragControlOffset;

        if (node is Node2D node2D)
        {
            _node2D = node2D;
        }
        else if (node is Control control)
        {
            _control = control;
        }
        else
        {
            throw new ArgumentException("Node must be either Node2D or Control");
        }
    }

    public Vector2 GlobalPosition
    {
        get
        {
            if (_node2D != null)
            {
                return _node2D.GlobalPosition;
            }
            else if (_control != null)
            {
                return _control.GlobalPosition;
            }
            else
            {
                throw new InvalidOperationException("Node is not initialized correctly");
            }
        }
        set
        {
            if (_node2D != null)
            {
                _node2D.GlobalPosition = value;
            }
            else if (_control != null)
            {
                _control.GlobalPosition = value;
            }
            else
            {
                throw new InvalidOperationException("Node is not initialized correctly");
            }
        }
    }

    public T GetParent<T>() where T : class
    {
        return _node2D?.GetParent() as T ?? _control?.GetParent() as T;
    }

    public void Reparent(IDraggableNode newParent)
    {
        if (newParent is Node parentNode)
        {
            if (_node2D != null)
            {
                _node2D.Reparent(parentNode);
            }
            else
            {
                _control?.Reparent(parentNode);
            }
        }
    }
}

public interface IDraggableNode
{
    Vector2 GlobalPosition { get; set; }

    T GetParent<T>() where T : class;
    void Reparent(IDraggableNode node);
}
