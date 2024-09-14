using Godot;
using System;
using System.Collections.Generic;
using Visualize.Utils;

namespace Visualize.Core;

public partial class VisualizeAutoload : Node
{
    private readonly Dictionary<ulong, VisualNodeInfo> nodeTrackers = new();

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
            (VBoxContainer vbox, List<Action> actions) = VisualUI.CreateVisualPanel(GetTree(), visualNode);
            ulong instanceId = node.GetInstanceId();

            Node positionalNode = GetClosestParentOfType(node, typeof(Node2D), typeof(Control));

            nodeTrackers.Add(instanceId, new VisualNodeInfo(actions, vbox, positionalNode ?? node));
        }
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
            VBoxContainer visualControl = info.VisualControl;

            // Update position based on node type
            if (node is Node2D node2D)
            {
                visualControl.GlobalPosition = node2D.GlobalPosition;
            }
            else if (node is Control control)
            {
                visualControl.GlobalPosition = control.GlobalPosition;
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
    public VBoxContainer VisualControl { get; }
    public Node Node { get; }

    public VisualNodeInfo(List<Action> actions, VBoxContainer visualControl, Node node)
    {
        Actions = actions ?? throw new ArgumentNullException(nameof(actions));
        VisualControl = visualControl ?? throw new ArgumentNullException(nameof(visualControl));
        Node = node ?? throw new ArgumentNullException(nameof(node));
    }
}
