using Godot;
using System;

namespace Visualize.Core;

public static partial class VisualControlTypes
{
    private static VisualControlInfo VisualVector4(object initialValue, Action<Vector4> valueChanged)
    {
        HBoxContainer vector4HBox = new();

        Vector4 vector4 = (Vector4)initialValue;

        SpinBox spinBoxX = CreateSpinBox(typeof(float));
        SpinBox spinBoxY = CreateSpinBox(typeof(float));
        SpinBox spinBoxZ = CreateSpinBox(typeof(float));
        SpinBox spinBoxW = CreateSpinBox(typeof(float));

        spinBoxX.Value = vector4.X;
        spinBoxY.Value = vector4.Y;
        spinBoxZ.Value = vector4.Z;
        spinBoxW.Value = vector4.W;

        spinBoxX.ValueChanged += value =>
        {
            vector4.X = (float)value;
            valueChanged(vector4);
        };

        spinBoxY.ValueChanged += value =>
        {
            vector4.Y = (float)value;
            valueChanged(vector4);
        };

        spinBoxZ.ValueChanged += value =>
        {
            vector4.Z = (float)value;
            valueChanged(vector4);
        };

        spinBoxW.ValueChanged += value =>
        {
            vector4.W = (float)value;
            valueChanged(vector4);
        };

        vector4HBox.AddChild(new Label { Text = "X" });
        vector4HBox.AddChild(spinBoxX);
        vector4HBox.AddChild(new Label { Text = "Y" });
        vector4HBox.AddChild(spinBoxY);
        vector4HBox.AddChild(new Label { Text = "Z" });
        vector4HBox.AddChild(spinBoxZ);
        vector4HBox.AddChild(new Label { Text = "W" });
        vector4HBox.AddChild(spinBoxW);

        return new VisualControlInfo(new Vector4Control(vector4HBox, spinBoxX, spinBoxY, spinBoxZ, spinBoxW));
    }
}

public class Vector4Control : IVisualControl
{
    private readonly HBoxContainer _vector4HBox;
    private readonly SpinBox _spinBoxX;
    private readonly SpinBox _spinBoxY;
    private readonly SpinBox _spinBoxZ;
    private readonly SpinBox _spinBoxW;

    public Vector4Control(HBoxContainer vector4HBox, SpinBox spinBoxX, SpinBox spinBoxY, SpinBox spinBoxZ, SpinBox spinBoxW)
    {
        _vector4HBox = vector4HBox;
        _spinBoxX = spinBoxX;
        _spinBoxY = spinBoxY;
        _spinBoxZ = spinBoxZ;
        _spinBoxW = spinBoxW;
    }

    public void SetValue(object value)
    {
        if (value is Vector4 vector4)
        {
            _spinBoxX.Value = vector4.X;
            _spinBoxY.Value = vector4.Y;
            _spinBoxZ.Value = vector4.Z;
            _spinBoxW.Value = vector4.W;
        }
    }

    public Control Control => _vector4HBox;

    public void SetEditable(bool editable)
    {
        _spinBoxX.Editable = editable;
        _spinBoxY.Editable = editable;
        _spinBoxZ.Editable = editable;
        _spinBoxW.Editable = editable;
    }
}
