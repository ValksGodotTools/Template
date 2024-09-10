using Godot;
using System;
using System.Collections.Generic;

namespace Visualize.Core;

public partial class VisualizeAutoload : Node
{
    private readonly Dictionary<ulong, VisualNodeInfo> nodeTrackers = new();

    public override void _Ready()
    {
        Window root = GetTree().Root;
        Godot.Collections.Array<Node> children = root.GetChildren();

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

            nodeTrackers.Add(instanceId, new VisualNodeInfo(actions, vbox, node));
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

    public override void _PhysicsProcess(double delta)
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
