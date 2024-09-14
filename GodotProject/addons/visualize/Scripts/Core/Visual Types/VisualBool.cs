using Godot;
using System;

namespace Visualize.Core;

public static partial class VisualControlTypes
{
    private static VisualControlInfo VisualBool(VisualControlContext context)
    {
        CheckBox checkBox = new() { ButtonPressed = (bool)context.InitialValue };
        checkBox.Toggled += value => context.ValueChanged(value);

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
