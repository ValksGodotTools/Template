using Godot;
using System;
using System.Collections.Generic;
using Visualize.Utils;

namespace Visualize.Core;

public static partial class VisualControlTypes
{
    private static VisualControlInfo VisualNumeric(object initialValue, Type type, List<VisualSpinBox> debugExportSpinBoxes, Action<object> valueChanged)
    {
        SpinBox spinBox = CreateSpinBox(type);

        spinBox.Value = Convert.ToDouble(initialValue);
        spinBox.ValueChanged += value =>
        {
            object convertedValue = Convert.ChangeType(value, type);
            valueChanged(convertedValue);
        };

        return new VisualControlInfo(new NumericControl(spinBox));
    }
}

public class NumericControl : IVisualControl
{
    private readonly SpinBox _spinBox;

    public NumericControl(SpinBox spinBox)
    {
        _spinBox = spinBox;
    }

    public void SetValue(object value)
    {
        if (value != null)
        {
            try
            {
                _spinBox.Value = Convert.ToDouble(value);
            }
            catch (InvalidCastException)
            {
                // Handle the case where the value cannot be converted to double
                PrintUtils.Warning($"Cannot convert value of type {value.GetType()} to double.");
            }
        }
    }

    public Control Control => _spinBox;

    public void SetEditable(bool editable)
    {
        _spinBox.Editable = editable;
    }
}
