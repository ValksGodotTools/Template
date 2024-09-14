using Godot;
using System;

namespace Visualize.Core;

public static partial class VisualControlTypes
{
    private static VisualControlInfo VisualVector3I(VisualControlContext context)
    {
        HBoxContainer vector3IHBox = new();

        Vector3I vector3I = (Vector3I)context.InitialValue;

        SpinBox spinBoxX = CreateSpinBox(typeof(int));
        SpinBox spinBoxY = CreateSpinBox(typeof(int));
        SpinBox spinBoxZ = CreateSpinBox(typeof(int));

        spinBoxX.Value = vector3I.X;
        spinBoxY.Value = vector3I.Y;
        spinBoxZ.Value = vector3I.Z;

        spinBoxX.ValueChanged += value =>
        {
            vector3I.X = (int)value;
            context.ValueChanged(vector3I);
        };

        spinBoxY.ValueChanged += value =>
        {
            vector3I.Y = (int)value;
            context.ValueChanged(vector3I);
        };

        spinBoxZ.ValueChanged += value =>
        {
            vector3I.Z = (int)value;
            context.ValueChanged(vector3I);
        };

        vector3IHBox.AddChild(new Label { Text = "X" });
        vector3IHBox.AddChild(spinBoxX);
        vector3IHBox.AddChild(new Label { Text = "Y" });
        vector3IHBox.AddChild(spinBoxY);
        vector3IHBox.AddChild(new Label { Text = "Z" });
        vector3IHBox.AddChild(spinBoxZ);

        return new VisualControlInfo(new Vector3IControl(vector3IHBox, spinBoxX, spinBoxY, spinBoxZ));
    }
}

public class Vector3IControl : IVisualControl
{
    private readonly HBoxContainer _vector3IHBox;
    private readonly SpinBox _spinBoxX;
    private readonly SpinBox _spinBoxY;
    private readonly SpinBox _spinBoxZ;

    public Vector3IControl(HBoxContainer vector3IHBox, SpinBox spinBoxX, SpinBox spinBoxY, SpinBox spinBoxZ)
    {
        _vector3IHBox = vector3IHBox;
        _spinBoxX = spinBoxX;
        _spinBoxY = spinBoxY;
        _spinBoxZ = spinBoxZ;
    }

    public void SetValue(object value)
    {
        if (value is Vector3I vector3I)
        {
            _spinBoxX.Value = vector3I.X;
            _spinBoxY.Value = vector3I.Y;
            _spinBoxZ.Value = vector3I.Z;
        }
    }

    public Control Control => _vector3IHBox;

    public void SetEditable(bool editable)
    {
        _spinBoxX.Editable = editable;
        _spinBoxY.Editable = editable;
        _spinBoxZ.Editable = editable;
    }
}
