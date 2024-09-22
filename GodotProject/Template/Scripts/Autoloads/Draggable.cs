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
    public static event Action<Node> DragReleased;

    private const float LERP_FACTOR = 0.3f;

    private Dictionary<Node, DragConstraints> _nodeDragConstraints = [];
    private Dictionary<Node, DragType> _nodeDragTypes = [];
    private HashSet<Node> _draggableNodes = [];

    private DraggableWrapper _selectedNode;
    private IDraggableNode _currentlyDraggedNode;
    private Node _previousParent;

    private Vector2 _previousPosition;
    private Vector2 _dragControlOffset;

    private bool _releaseClickAfterDrag;

    public override void _Ready()
    {
        MakeNodesDraggable();
        GetTree().NodeAdded += TryMakeNodeDraggable;
        SetPhysicsProcess(false);
    }

    public override void _Input(InputEvent @event)
    {
        if (@event is InputEventMouseButton btn)
        {
            bool draggingNodeInClickMode = _selectedNode != null && _nodeDragTypes[_selectedNode.Node] == DragType.Click && _releaseClickAfterDrag;

            if (btn.IsLeftClickPressed() && !draggingNodeInClickMode)
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
                    _previousParent = _selectedNode.GetParent();
                    _previousPosition = _selectedNode.GlobalPosition;

                    // Reparent to viewport root
                    _selectedNode.Node.Reparent(GetTree().Root);

                    _currentlyDraggedNode = _selectedNode;
                    SetPhysicsProcess(true);
                }
            }

            if (btn.IsLeftClickReleased() && _currentlyDraggedNode != null)
            {
                if (_nodeDragTypes[_selectedNode.Node] == DragType.Hold)
                {
                    HandleReleaseDraggableNode();
                }
                else if (_nodeDragTypes[_selectedNode.Node] == DragType.Click)
                {
                    if (_releaseClickAfterDrag)
                    {
                        _releaseClickAfterDrag = false;

                        HandleReleaseDraggableNode();
                        return;
                    }

                    _releaseClickAfterDrag = true;
                }
            }
        }
    }

    public override void _PhysicsProcess(double delta)
    {
        if (_currentlyDraggedNode != null)
        {
            // Only use MoveTowards for Node2D's as using on Control's looks and feels weird
            if (_currentlyDraggedNode.Node is Node2D)
            {
                float distance = _currentlyDraggedNode.GlobalPosition.DistanceTo(GetGlobalMousePosition());

                _currentlyDraggedNode.GlobalPosition = _currentlyDraggedNode.GlobalPosition
                    .MoveToward(GetGlobalMousePosition() - _dragControlOffset, distance * LERP_FACTOR);
            }
            else
            {
                _currentlyDraggedNode.GlobalPosition = GetGlobalMousePosition() - _dragControlOffset;
            }
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
        // An Area2D has been created for this node already
        if (_draggableNodes.Contains(node))
        {
            return;
        }

        DraggableAttribute attribute = node.GetType().GetCustomAttribute<DraggableAttribute>();

        if (attribute == null)
        {
            return;
        }

        _nodeDragTypes.Add(node, attribute.DragType);
        _nodeDragConstraints.Add(node, attribute.DragConstraints);

        Area2D area = CreateDraggableArea(node);

        node.AddChild(area);

        _draggableNodes.Add(node);
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

    private void HandleReleaseDraggableNode()
    {
        SetPhysicsProcess(false);

        // Expose event to developers to let them do things with node
        DragReleased?.Invoke(_selectedNode.Node);

        // Developer has not queue freed the node
        bool valid = IsInstanceValid(_selectedNode.Node);

        // Developer has not reparented the node
        bool sameParent = _selectedNode.GetParent() == GetTree().Root;

        // Since node was not queue freed or reparented, snap it back to its previous parent
        if (valid && sameParent)
        {
            _selectedNode.Reparent(_previousParent);
            _selectedNode.GlobalPosition = _previousPosition;
        }

        _currentlyDraggedNode = null;
        _selectedNode = null;
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
    public Node Node { get; set; }

    private Node2D _node2D;
    private Control _control;

    public DraggableWrapper(Node node, Vector2 dragControlOffset)
    {
        Node = node;
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

    public Node GetParent()
    {
        return _node2D?.GetParent() ?? _control?.GetParent();
    }

    public void Reparent(Node newParent)
    {
        if (_node2D != null)
        {
            _node2D.Reparent(newParent);
        }
        else
        {
            _control?.Reparent(newParent);
        }
    }
}

public interface IDraggableNode
{
    Node Node { get; set; }
    Vector2 GlobalPosition { get; set; }

    Node GetParent();
    void Reparent(Node node);
}
