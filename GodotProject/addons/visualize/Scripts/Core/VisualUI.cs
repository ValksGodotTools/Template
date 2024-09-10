using Godot;
using System;
using System.Collections.Generic;
using System.Reflection;
using Visualize.Utils;
using static Godot.Control;

namespace Visualize.Core;

public static class VisualUI
{
    public const float VISUAL_UI_SCALE_FACTOR = 0.6f;

    public static (VBoxContainer, List<Action>) CreateVisualPanel(SceneTree tree, VisualNode debugVisualNode)
    {
        List<VisualSpinBox> debugExportSpinBoxes = new();
        Dictionary<Node, VBoxContainer> visualNodes = new();
        List<Action> updateControls = new();

        Node node = debugVisualNode.Node;

        VBoxContainer vboxMembers = CreateVisualContainer(node.Name);

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

                Type memberType = property != null ? property.PropertyType : field.FieldType;

                VisualControlInfo visualControlInfo = VisualControlTypes.CreateControlForType(initialValue, memberType, debugExportSpinBoxes, v =>
                {
                    // Do nothing
                });

                visualControlInfo.Control.SetEditable(false);

                updateControls.Add(() =>
                {
                    object newValue = property != null
                        ? property.GetValue(property.GetGetMethod(true).IsStatic ? null : node)
                        : field.GetValue(field.IsStatic ? null : node);

                    visualControlInfo.Control.SetValue(newValue);
                });

                HBoxContainer hbox = new();
                hbox.AddChild(new Label { Text = visualMember });
                hbox.AddChild(visualControlInfo.Control.Control);

                vboxMembers.AddChild(hbox);
            }
        }

        AddMemberInfoElements(vboxMembers, debugVisualNode.Properties, node, debugExportSpinBoxes);

        AddMemberInfoElements(vboxMembers, debugVisualNode.Fields, node, debugExportSpinBoxes);

        VisualMethods.AddMethodInfoElements(vboxMembers, debugVisualNode.Methods, node, debugExportSpinBoxes);

        VBoxContainer vboxLogs = new();
        vboxMembers.AddChild(vboxLogs);

        visualNodes.Add(node, vboxLogs);

        // Add vbox to scene tree to get vbox.Size for later
        tree.Root.AddChild(vboxMembers);

        // All debug UI elements should not be influenced by the game world environments lighting
        node.GetChildren<Control>().ForEach(child => child.SetUnshaded());

        vboxMembers.Scale = Vector2.One * VISUAL_UI_SCALE_FACTOR;

        if (debugVisualNode.InitialPosition != Vector2.Zero)
        {
            vboxMembers.GlobalPosition = debugVisualNode.InitialPosition;
        }

        // This is ugly but I don't know how else to do it
        VisualLogger.VisualNodes = visualNodes;

        return (vboxMembers, updateControls);
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

    private static void AddMemberInfoElements(VBoxContainer vbox, IEnumerable<MemberInfo> members, Node node, List<VisualSpinBox> debugExportSpinBoxes)
    {
        foreach (MemberInfo member in members)
        {
            Control element = CreateMemberInfoElement(member, node, debugExportSpinBoxes);
            vbox.AddChild(element);
        }
    }

    private static HBoxContainer CreateMemberInfoElement(MemberInfo member, Node node, List<VisualSpinBox> debugExportSpinBoxes)
    {
        HBoxContainer hbox = new();

        Type type = VisualHandler.GetMemberType(member);

        object initialValue = VisualHandler.GetMemberValue(member, node);

        VisualControlInfo element = VisualControlTypes.CreateControlForType(initialValue, type, debugExportSpinBoxes, v =>
        {
            VisualHandler.SetMemberValue(member, node, v);
        });

        if (element.Control.Control != null)
        {
            Label label = new()
            {
                Text = member.Name.ToPascalCase().AddSpaceBeforeEachCapital(),
                SizeFlagsHorizontal = SizeFlags.ExpandFill
            };

            hbox.AddChild(label);
            hbox.AddChild(element.Control.Control);
        }

        return hbox;
    }
}
