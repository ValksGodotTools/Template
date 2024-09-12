using Godot;
using System;

namespace Visualize.Core;

public static partial class VisualControlTypes
{
    private static VisualControlInfo VisualBool(object initialValue, Action<bool> valueChanged)
    {
        CheckBox checkBox = new() { ButtonPressed = (bool)initialValue };
        checkBox.Toggled += value => valueChanged(value);

        return new VisualControlInfo(new BoolControl(checkBox));
    }
}

public class BoolControl : IVisualControl
{
    private readonly CheckBox _checkBox;

    public BoolControl(CheckBox checkBox)
    {
        _checkBox = checkBox;
    }

    public void SetValue(object value)
    {
        if (value is bool boolValue)
        {
            _checkBox.ButtonPressed = boolValue;
        }
    }

    public Control Control => _checkBox;

    public void SetEditable(bool editable)
    {
        _checkBox.Disabled = !editable;
    }
}
