using Godot;
using System;

namespace Visualize.Core;

public static partial class VisualControlTypes
{
    private static VisualControlInfo VisualStringName(object initialValue, Action<StringName> valueChanged)
    {
        StringName stringName = (StringName)initialValue;
        string initialText = stringName != null ? stringName.ToString() : string.Empty;

        LineEdit lineEdit = new() { Text = initialText };
        lineEdit.TextChanged += text => valueChanged(new StringName(text));

        return new VisualControlInfo(new LineEditControl(lineEdit));
    }
}
