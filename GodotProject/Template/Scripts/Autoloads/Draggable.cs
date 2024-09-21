using Godot;
using GodotUtils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Template;

// Autoload
public partial class Draggable : Node2D
{
    private const float LERP_FACTOR = 0.2f;

    private DraggableWrapper _selectedNode;
    private IDraggableNode _previousParent;
    private Vector2 _previousPosition;
    private Vector2 _dragControlOffset;
    private IDraggableNode _currentlyDraggedNode;

    public override void _Ready()
    {
        MakeNodesDraggable();
        GetTree().NodeAdded += TryMakeNodeDraggable;
    }

    public override void _Input(InputEvent @event)
    {
        if (@event is InputEventMouseButton btn)
        {
            if (btn.IsLeftClickPressed())
            {
                // Consider the scenario where the cursor does not leave the Area2D area when
                // letting go of a draggable. The area.MouseEntered event will never get fired
                // because the cursor is already in the area. So below is the code to handle this,
                // that is when _selectedNode is null.
                if (_selectedNode == null)
                {
                    Node node = GetNodeUnderCursor(GetWorld2D(), GetGlobalMousePosition());

                    if (node is Area2D)
                    {
                        Node parent = node.GetParent();

                        if (parent.GetType().GetCustomAttribute<DraggableAttribute>() != null)
                        {
                            Vector2 dragControlOffset = Vector2.Zero;
                            Vector2 size = GetNodeSize(parent);

                            if (parent is Control)
                            {
                                dragControlOffset = size * 0.5f;
                            }

                            _selectedNode = new DraggableWrapper(parent, dragControlOffset);
                        }
                    }
                }

                // The area.MouseEntered event has populated the selected node, lets prepare
                // the node for dragging
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
            float distance = _currentlyDraggedNode.GlobalPosition.DistanceTo(GetGlobalMousePosition());

            _currentlyDraggedNode.GlobalPosition = _currentlyDraggedNode.GlobalPosition
                .MoveToward(GetGlobalMousePosition() - _dragControlOffset, distance * LERP_FACTOR);
        }
    }

    private void MakeNodesDraggable()
    {
        IEnumerable<Node> nodes = GetTree().Root.GetChildren<Node>().Where(n => n is Node2D or Control);

        foreach (Node node in nodes)
        {
            TryMakeNodeDraggable(node);
        }
    }

    private void TryMakeNodeDraggable(Node node)
    {
        DraggableAttribute attribute = node.GetType().GetCustomAttribute<DraggableAttribute>();

        if (attribute == null)
        {
            return;
        }

        Area2D area = CreateDraggableArea(node);

        node.AddChild(area);
    }

    private Area2D CreateDraggableArea(Node node)
    {
        Area2D area = new();

        Vector2 dragControlOffset = Vector2.Zero;
        Vector2 size = GetNodeSize(node);

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

        return area;
    }

    private static Vector2 GetNodeSize(Node node)
    {
        Vector2 size = Vector2.Zero;

        if (node is Sprite2D sprite)
        {
            size = sprite.GetScaledSize();
        }
        else if (node is AnimatedSprite2D animatedSprite)
        {
            size = animatedSprite.GetScaledSize();
        }
        else if (node is Control control)
        {
            size = control.Size * control.Scale;
        }

        return size;
    }

    private static Node GetNodeUnderCursor(World2D world, Vector2 cursorPosition)
    {
        // Create a shape query parameters object
        PhysicsShapeQueryParameters2D queryParams = new();
        queryParams.Transform = new Transform2D(0, cursorPosition);
        queryParams.CollideWithAreas = true;

        // Use a small circle shape to simulate a point intersection
        CircleShape2D circleShape = new();
        circleShape.Radius = 1.0f;
        queryParams.Shape = circleShape;

        // Perform the query
        PhysicsDirectSpaceState2D spaceState =
            PhysicsServer2D.SpaceGetDirectState(world.GetSpace());

        Godot.Collections.Array<Godot.Collections.Dictionary> results =
            spaceState.IntersectShape(queryParams, 1);

        if (results.Count > 0)
        {
            Godot.Collections.Dictionary result = results[0];

            if (result != null && result.ContainsKey("collider"))
            {
                return result["collider"].As<Node>();
            }
        }

        return null;
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
