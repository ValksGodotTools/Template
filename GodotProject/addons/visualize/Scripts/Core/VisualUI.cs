using Godot;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using System.Threading;
using Visualize.Utils;
using static Godot.Control;

namespace Visualize.Core;

public static class VisualUI
{
    public const float VISUAL_UI_SCALE_FACTOR = 0.6f;

    public static (VBoxContainer, List<Action>) CreateVisualPanel(SceneTree tree, VisualNode debugVisualNode)
    {
        List<VisualSpinBox> spinBoxes = new();
        Dictionary<Node, VBoxContainer> visualNodes = new();
        List<Action> updateControls = new();

        Node node = debugVisualNode.Node;

        VBoxContainer vboxParent = new();

        VBoxContainer vboxMembers = CreateVisualContainer(node.Name);

        VBoxContainer readonlyMembers = new();

        string[] visualizeMembers = debugVisualNode.VisualizeMembers;

        if (visualizeMembers != null)
        {
            foreach (string visualMember in visualizeMembers)
            {
                // Try to get the property first
                PropertyInfo property = node.GetType().GetProperty(visualMember, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance);
                FieldInfo field = null;
                object initialValue = null;

                if (property != null)
                {
                    initialValue = property.GetValue(property.GetGetMethod(true).IsStatic ? null : node);
                }
                else
                {
                    // If property is null, try to get the field
                    field = node.GetType().GetField(visualMember, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance);
                    if (field != null)
                    {
                        initialValue = field.GetValue(field.IsStatic ? null : node);
                    }
                }

                // If neither property nor field is found, skip this member
                if (property == null && field == null)
                {
                    continue;
                }

                if (initialValue != null)
                {
                    AddVisualControl(visualMember, readonlyMembers, node, field, property, initialValue, updateControls, spinBoxes);
                }
                else
                {
                    _ = TryAddVisualControlAsync(visualMember, readonlyMembers, node, field, property, updateControls, spinBoxes);
                }
            }
        }

        AddMemberInfoElements(vboxMembers, debugVisualNode.Properties, node, spinBoxes);

        AddMemberInfoElements(vboxMembers, debugVisualNode.Fields, node, spinBoxes);

        VisualMethods.AddMethodInfoElements(vboxMembers, debugVisualNode.Methods, node, spinBoxes);

        VBoxContainer vboxLogs = new();
        vboxMembers.AddChild(vboxLogs);

        visualNodes.Add(node, vboxLogs);

        vboxParent.AddChild(readonlyMembers);
        vboxParent.AddChild(vboxMembers);

        foreach (BaseButton baseButton in vboxParent.GetChildren<BaseButton>())
        {
            baseButton.Pressed += () =>
            {
                _ = new GTween(baseButton)
                    .Delay(0.1)
                    .Callback(() => baseButton.ReleaseFocus());
            };
        }

        vboxMembers.Modulate = new Color(0.8f, 1, 0.8f, 1);

        // Add to canvas layer so UI is not affected by lighting in game world
        CanvasLayer canvasLayer = new();
        canvasLayer.FollowViewportEnabled = true;
        canvasLayer.AddChild(vboxParent);

        tree.Root.CallDeferred(Node.MethodName.AddChild, canvasLayer);

        vboxParent.Scale = Vector2.One * VISUAL_UI_SCALE_FACTOR;

        if (debugVisualNode.InitialPosition != Vector2.Zero)
        {
            vboxMembers.GlobalPosition = debugVisualNode.InitialPosition;
        }

        // This is ugly but I don't know how else to do it
        VisualLogger.VisualNodes = visualNodes;

        vboxParent.Name = node.Name;

        return (vboxParent, updateControls);
    }

    private static async Task TryAddVisualControlAsync(string visualMember, VBoxContainer readonlyMembers, Node node, FieldInfo field, PropertyInfo property, List<Action> updateControls, List<VisualSpinBox> spinBoxes)
    {
        CancellationTokenSource cts = new();
        CancellationToken token = cts.Token;

        int elapsedSeconds = 0;

        while (!token.IsCancellationRequested)
        {
            object value = null;

            if (field != null)
            {
                value = field.GetValue(node);
            }
            else if (property != null)
            {
                value = property.GetValue(node);
            }

            if (value != null)
            {
                AddVisualControl(visualMember, readonlyMembers, node, field, property, value, updateControls, spinBoxes);
                break;
            }

            try
            {
                await Task.Delay(1000, token);
                elapsedSeconds++;

                if (elapsedSeconds == 3)
                {
                    string memberName = string.Empty;

                    if (field != null)
                    {
                        memberName = field.Name;
                    }
                    else if (property != null)
                    {
                        memberName = property.Name;
                    }

                    GD.PrintRich($"[color=orange]Tracking '{node.Name}' to see if '{memberName}' value changes[/color]");
                }
            }
            catch (TaskCanceledException)
            {
                // Task was cancelled, exit the loop
                break;
            }
        }
    }

    private static void AddVisualControl(string visualMember, VBoxContainer readonlyMembers, Node node, FieldInfo field, PropertyInfo property, object initialValue, List<Action> updateControls, List<VisualSpinBox> spinBoxes)
    {
        Type memberType = property != null ? property.PropertyType : field.FieldType;

        VisualControlContext context = new(spinBoxes, initialValue, v =>
        {
            // Do nothing
        });

        VisualControlInfo visualControlInfo = VisualControlTypes.CreateControlForType(memberType, context);

        visualControlInfo.VisualControl.SetEditable(false);

        updateControls.Add(() =>
        {
            object newValue = property != null
                ? property.GetValue(property.GetGetMethod(true).IsStatic ? null : node)
                : field.GetValue(field.IsStatic ? null : node);

            visualControlInfo.VisualControl.SetValue(newValue);
        });

        HBoxContainer hbox = new() { Modulate = new Color(1.0f, 0.75f, 0.8f, 1) };
        hbox.AddChild(new Label { Text = visualMember });
        hbox.AddChild(visualControlInfo.VisualControl.Control);
        readonlyMembers.AddChild(hbox);
    }

    private static VBoxContainer CreateVisualContainer(string nodeName)
    {
        VBoxContainer vbox = new()
        {
            // Ensure this info is rendered above all game elements
            ZIndex = (int)RenderingServer.CanvasItemZMax
        };

        Label label = new() { Text = nodeName };

        vbox.AddChild(label);

        return vbox;
    }

    private static void AddMemberInfoElements(VBoxContainer vbox, IEnumerable<MemberInfo> members, Node node, List<VisualSpinBox> spinBoxes)
    {
        foreach (MemberInfo member in members)
        {
            Control element = CreateMemberInfoElement(member, node, spinBoxes);
            vbox.AddChild(element);
        }
    }

    private static HBoxContainer CreateMemberInfoElement(MemberInfo member, Node node, List<VisualSpinBox> spinBoxes)
    {
        HBoxContainer hbox = new();

        Type type = VisualHandler.GetMemberType(member);

        object initialValue = VisualHandler.GetMemberValue(member, node);

        VisualControlInfo element = VisualControlTypes.CreateControlForType(type, new VisualControlContext(spinBoxes, initialValue, v => 
        {
            VisualHandler.SetMemberValue(member, node, v);
        }));

        if (element.VisualControl != null)
        {
            Label label = new()
            {
                Text = member.Name.ToPascalCase().AddSpaceBeforeEachCapital(),
                SizeFlagsHorizontal = SizeFlags.ExpandFill
            };

            hbox.AddChild(label);
            hbox.AddChild(element.VisualControl.Control);
        }

        return hbox;
    }
}
