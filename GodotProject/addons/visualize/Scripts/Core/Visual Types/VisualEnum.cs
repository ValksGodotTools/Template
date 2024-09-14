using Godot;
using System;
using Visualize.Utils;

namespace Visualize.Core;

public static partial class VisualControlTypes
{
    private static VisualControlInfo VisualEnum(Type type, VisualControlContext context)
    {
        GOptionButtonEnum optionButton = new(type);
        optionButton.Select(context.InitialValue);
        optionButton.OnItemSelected += item => context.ValueChanged(item);

        return new VisualControlInfo(new OptionButtonEnumControl(optionButton));
    }
}

public class OptionButtonEnumControl : IVisualControl
{
    private readonly GOptionButtonEnum _optionButton;

    public OptionButtonEnumControl(GOptionButtonEnum optionButton)
    {
        _optionButton = optionButton;
    }

    public void SetValue(object value)
    {
        _optionButton.Select(value);
    }

    public Control Control => _optionButton.Control;

    public void SetEditable(bool editable)
    {
        _optionButton.Control.Disabled = !editable;
    }
}
