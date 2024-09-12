using Godot;
using System;

namespace Visualize.Core;

public static partial class VisualControlTypes
{
    private static VisualControlInfo VisualObject(object initialValue, Action<object> valueChanged)
    {
        LineEdit lineEdit = new() { Text = initialValue?.ToString() ?? string.Empty };
        lineEdit.TextChanged += text => valueChanged(text);

        return new VisualControlInfo(new LineEditControl(lineEdit));
    }
}
