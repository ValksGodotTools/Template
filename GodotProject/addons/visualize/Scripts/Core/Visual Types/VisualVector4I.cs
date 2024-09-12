using Godot;
using System;

namespace Visualize.Core;

public static partial class VisualControlTypes
{
    private static VisualControlInfo VisualVector4I(object initialValue, Action<Vector4I> valueChanged)
    {
        HBoxContainer vector4IHBox = new();

        Vector4I vector4I = (Vector4I)initialValue;

        SpinBox spinBoxX = CreateSpinBox(typeof(int));
        SpinBox spinBoxY = CreateSpinBox(typeof(int));
        SpinBox spinBoxZ = CreateSpinBox(typeof(int));
        SpinBox spinBoxW = CreateSpinBox(typeof(int));

        spinBoxX.Value = vector4I.X;
        spinBoxY.Value = vector4I.Y;
        spinBoxZ.Value = vector4I.Z;
        spinBoxW.Value = vector4I.W;

        spinBoxX.ValueChanged += value =>
        {
            vector4I.X = (int)value;
            valueChanged(vector4I);
        };

        spinBoxY.ValueChanged += value =>
        {
            vector4I.Y = (int)value;
            valueChanged(vector4I);
        };

        spinBoxZ.ValueChanged += value =>
        {
            vector4I.Z = (int)value;
            valueChanged(vector4I);
        };

        spinBoxW.ValueChanged += value =>
        {
            vector4I.W = (int)value;
            valueChanged(vector4I);
        };

        vector4IHBox.AddChild(new Label { Text = "X" });
        vector4IHBox.AddChild(spinBoxX);
        vector4IHBox.AddChild(new Label { Text = "Y" });
        vector4IHBox.AddChild(spinBoxY);
        vector4IHBox.AddChild(new Label { Text = "Z" });
        vector4IHBox.AddChild(spinBoxZ);
        vector4IHBox.AddChild(new Label { Text = "W" });
        vector4IHBox.AddChild(spinBoxW);

        return new VisualControlInfo(new Vector4IControl(vector4IHBox, spinBoxX, spinBoxY, spinBoxZ, spinBoxW));
    }
}

public class Vector4IControl : IVisualControl
{
    private readonly HBoxContainer _vector4IHBox;
    private readonly SpinBox _spinBoxX;
    private readonly SpinBox _spinBoxY;
    private readonly SpinBox _spinBoxZ;
    private readonly SpinBox _spinBoxW;

    public Vector4IControl(HBoxContainer vector4IHBox, SpinBox spinBoxX, SpinBox spinBoxY, SpinBox spinBoxZ, SpinBox spinBoxW)
    {
        _vector4IHBox = vector4IHBox;
        _spinBoxX = spinBoxX;
        _spinBoxY = spinBoxY;
        _spinBoxZ = spinBoxZ;
        _spinBoxW = spinBoxW;
    }

    public void SetValue(object value)
    {
        if (value is Vector4I vector4I)
        {
            _spinBoxX.Value = vector4I.X;
            _spinBoxY.Value = vector4I.Y;
            _spinBoxZ.Value = vector4I.Z;
            _spinBoxW.Value = vector4I.W;
        }
    }

    public Control Control => _vector4IHBox;

    public void SetEditable(bool editable)
    {
        _spinBoxX.Editable = editable;
        _spinBoxY.Editable = editable;
        _spinBoxZ.Editable = editable;
        _spinBoxW.Editable = editable;
    }
}
