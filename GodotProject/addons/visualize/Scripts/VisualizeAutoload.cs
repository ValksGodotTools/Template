using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using Visualize.Utils;

namespace Visualize.Core;

public partial class VisualizeAutoload : Node
{
    private readonly Dictionary<ulong, VisualNodeInfo> nodeTrackers = [];

    public override void _Ready()
    {
        Window root = GetTree().Root;
        List<Node> children = root.GetChildren<Node>();

        foreach (Node node in children)
        {
            AddVisualNode(node);
        }

        GetTree().NodeAdded += AddVisualNode;
        GetTree().NodeRemoved += RemoveVisualNode;
    }

    private void AddVisualNode(Node node)
    {
        VisualNode visualNode = VisualizeAttributeHandler.RetrieveData(node);

        if (visualNode != null)
        {
            (Control visualPanel, List<Action> actions) = VisualUI.CreateVisualPanel(GetTree(), visualNode);
            ulong instanceId = node.GetInstanceId();

            Node positionalNode = GetClosestParentOfType(node, typeof(Node2D), typeof(Control));

            if (positionalNode == null)
            {
                PrintUtils.Warning($"No positional parent node could be found for {node.Name} so no VisualPanel will be created for it");
                return;
            }

            // Immediately set the visual panels position to the positional nodes position
            if (positionalNode is Node2D node2D)
            {
                visualPanel.GlobalPosition = node2D.GlobalPosition;
            }
            else if (positionalNode is Control control)
            {
                visualPanel.GlobalPosition = control.GlobalPosition;
            }

            // Ensure the added visual panel is not overlapping with any other visual panels
            IEnumerable<Control> controls = nodeTrackers.Select(x => x.Value.VisualControl);

            Vector2 offset = Vector2.Zero;

            foreach (Control existingControl in controls)
            {
                if (existingControl == visualPanel)
                    continue; // Skip checking against itself

                if (ControlsOverlapping(visualPanel, existingControl))
                {
                    // Move vbox down by the existing controls height
                    offset += new Vector2(0, existingControl.GetRect().Size.Y);
                }
            }

            nodeTrackers.Add(instanceId, new VisualNodeInfo(actions, visualPanel, positionalNode ?? node, offset));
        }
    }

    private static bool ControlsOverlapping(Control control1, Control control2)
    {
        // Get the bounding rectangles of the control nodes
        Rect2 rect1 = control1.GetRect();
        Rect2 rect2 = control2.GetRect();

        // Check if the rectangles intersect
        return rect1.Intersects(rect2);
    }

    private void RemoveVisualNode(Node node)
    {
        ulong instanceId = node.GetInstanceId();

        if (nodeTrackers.TryGetValue(instanceId, out VisualNodeInfo info))
        {
            info.VisualControl.QueueFree();
            nodeTrackers.Remove(instanceId);
        }
    }

    public override void _Process(double delta)
    {
        foreach (KeyValuePair<ulong, VisualNodeInfo> kvp in nodeTrackers)
        {
            VisualNodeInfo info = kvp.Value;
            Node node = info.Node;
            Control visualControl = info.VisualControl;

            // Update position based on node type
            if (node is Node2D node2D)
            {
                visualControl.GlobalPosition = node2D.GlobalPosition + info.Offset;
            }
            else if (node is Control control)
            {
                visualControl.GlobalPosition = control.GlobalPosition + info.Offset;
            }

            // Execute actions
            foreach (Action action in info.Actions)
            {
                action();
            }
        }
    }

    private static Node GetClosestParentOfType(Node node, params Type[] typesToCheck)
    {
        // Check if the current node is of one of the specified types
        if (IsNodeOfType(node, typesToCheck))
        {
            return node;
        }

        // Recursively get the parent and check its type
        Node parent = node.GetParent();

        while (parent != null)
        {
            if (IsNodeOfType(parent, typesToCheck))
            {
                return parent;
            }

            parent = parent.GetParent();
        }

        // If no suitable parent is found, return null
        return null;
    }

    private static bool IsNodeOfType(Node node, Type[] typesToCheck)
    {
        foreach (Type type in typesToCheck)
        {
            if (type.IsInstanceOfType(node))
            {
                return true;
            }
        }

        return false;
    }
}

public class VisualNodeInfo
{
    public List<Action> Actions { get; }
    public Control VisualControl { get; }
    public Vector2 Offset { get; }
    public Node Node { get; }

    public VisualNodeInfo(List<Action> actions, Control visualControl, Node node, Vector2 offset)
    {
        Actions = actions ?? throw new ArgumentNullException(nameof(actions));
        VisualControl = visualControl ?? throw new ArgumentNullException(nameof(visualControl));
        Node = node ?? throw new ArgumentNullException(nameof(node));
        Offset = offset;
    }
}
