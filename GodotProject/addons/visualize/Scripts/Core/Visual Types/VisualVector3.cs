using Godot;
using System;

namespace Visualize.Core;

public static partial class VisualControlTypes
{
    private static VisualControlInfo VisualVector3(object initialValue, Action<Vector3> valueChanged)
    {
        HBoxContainer vector3HBox = new();

        Vector3 vector3 = (Vector3)initialValue;

        SpinBox spinBoxX = CreateSpinBox(typeof(float));
        SpinBox spinBoxY = CreateSpinBox(typeof(float));
        SpinBox spinBoxZ = CreateSpinBox(typeof(float));

        spinBoxX.Value = vector3.X;
        spinBoxY.Value = vector3.Y;
        spinBoxZ.Value = vector3.Z;

        spinBoxX.ValueChanged += value =>
        {
            vector3.X = (float)value;
            valueChanged(vector3);
        };

        spinBoxY.ValueChanged += value =>
        {
            vector3.Y = (float)value;
            valueChanged(vector3);
        };

        spinBoxZ.ValueChanged += value =>
        {
            vector3.Z = (float)value;
            valueChanged(vector3);
        };

        vector3HBox.AddChild(new Label { Text = "X" });
        vector3HBox.AddChild(spinBoxX);
        vector3HBox.AddChild(new Label { Text = "Y" });
        vector3HBox.AddChild(spinBoxY);
        vector3HBox.AddChild(new Label { Text = "Z" });
        vector3HBox.AddChild(spinBoxZ);

        return new VisualControlInfo(new Vector3Control(vector3HBox, spinBoxX, spinBoxY, spinBoxZ));
    }
}

public class Vector3Control : IVisualControl
{
    private readonly HBoxContainer _vector3HBox;
    private readonly SpinBox _spinBoxX;
    private readonly SpinBox _spinBoxY;
    private readonly SpinBox _spinBoxZ;

    public Vector3Control(HBoxContainer vector3HBox, SpinBox spinBoxX, SpinBox spinBoxY, SpinBox spinBoxZ)
    {
        _vector3HBox = vector3HBox;
        _spinBoxX = spinBoxX;
        _spinBoxY = spinBoxY;
        _spinBoxZ = spinBoxZ;
    }

    public void SetValue(object value)
    {
        if (value is Vector3 vector3)
        {
            _spinBoxX.Value = vector3.X;
            _spinBoxY.Value = vector3.Y;
            _spinBoxZ.Value = vector3.Z;
        }
    }

    public Control Control => _vector3HBox;

    public void SetEditable(bool editable)
    {
        _spinBoxX.Editable = editable;
        _spinBoxY.Editable = editable;
        _spinBoxZ.Editable = editable;
    }
}
