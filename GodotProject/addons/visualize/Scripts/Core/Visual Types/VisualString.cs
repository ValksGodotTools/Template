using Godot;
using System;

namespace Visualize.Core;

public static partial class VisualControlTypes
{
    private static VisualControlInfo VisualString(object initialValue, Action<string> valueChanged)
    {
        LineEdit lineEdit = new() { Text = initialValue.ToString() };
        lineEdit.TextChanged += text => valueChanged(text);

        return new VisualControlInfo(new LineEditControl(lineEdit));
    }
}

public class LineEditControl : IVisualControl
{
    private readonly LineEdit _lineEdit;

    public LineEditControl(LineEdit lineEdit)
    {
        _lineEdit = lineEdit;
    }

    public void SetValue(object value)
    {
        if (value is string text)
        {
            _lineEdit.Text = text;
        }
    }

    public Control Control => _lineEdit;

    public void SetEditable(bool editable)
    {
        _lineEdit.Editable = editable;
    }
}
