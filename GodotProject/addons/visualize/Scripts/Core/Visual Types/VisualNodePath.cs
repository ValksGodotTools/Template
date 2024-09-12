using Godot;
using System;

namespace Visualize.Core;

public static partial class VisualControlTypes
{
    private static VisualControlInfo VisualNodePath(object initialValue, Action<NodePath> valueChanged)
    {
        NodePath nodePath = (NodePath)initialValue;
        string initialText = nodePath != null ? nodePath.ToString() : string.Empty;

        LineEdit lineEdit = new() { Text = initialText };
        lineEdit.TextChanged += text => valueChanged(new NodePath(text));

        return new VisualControlInfo(new LineEditControl(lineEdit));
    }
}
