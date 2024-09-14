using Godot;
using System;

namespace Visualize.Core;

public static partial class VisualControlTypes
{
    private static VisualControlInfo VisualQuaternion(VisualControlContext context)
    {
        HBoxContainer quaternionHBox = new();

        Quaternion quaternion = (Quaternion)context.InitialValue;

        SpinBox spinBoxX = CreateSpinBox(typeof(float));
        SpinBox spinBoxY = CreateSpinBox(typeof(float));
        SpinBox spinBoxZ = CreateSpinBox(typeof(float));
        SpinBox spinBoxW = CreateSpinBox(typeof(float));

        spinBoxX.Value = quaternion.X;
        spinBoxY.Value = quaternion.Y;
        spinBoxZ.Value = quaternion.Z;
        spinBoxW.Value = quaternion.W;

        spinBoxX.ValueChanged += value =>
        {
            quaternion.X = (float)value;
            context.ValueChanged(quaternion);
        };

        spinBoxY.ValueChanged += value =>
        {
            quaternion.Y = (float)value;
            context.ValueChanged(quaternion);
        };

        spinBoxZ.ValueChanged += value =>
        {
            quaternion.Z = (float)value;
            context.ValueChanged(quaternion);
        };

        spinBoxW.ValueChanged += value =>
        {
            quaternion.W = (float)value;
            context.ValueChanged(quaternion);
        };

        quaternionHBox.AddChild(new Label { Text = "X" });
        quaternionHBox.AddChild(spinBoxX);
        quaternionHBox.AddChild(new Label { Text = "Y" });
        quaternionHBox.AddChild(spinBoxY);
        quaternionHBox.AddChild(new Label { Text = "Z" });
        quaternionHBox.AddChild(spinBoxZ);
        quaternionHBox.AddChild(new Label { Text = "W" });
        quaternionHBox.AddChild(spinBoxW);

        return new VisualControlInfo(new QuaternionControl(quaternionHBox, spinBoxX, spinBoxY, spinBoxZ, spinBoxW));
    }
}

public class QuaternionControl : IVisualControl
{
    private readonly HBoxContainer _quaternionHBox;
    private readonly SpinBox _spinBoxX;
    private readonly SpinBox _spinBoxY;
    private readonly SpinBox _spinBoxZ;
    private readonly SpinBox _spinBoxW;

    public QuaternionControl(HBoxContainer quaternionHBox, SpinBox spinBoxX, SpinBox spinBoxY, SpinBox spinBoxZ, SpinBox spinBoxW)
    {
        _quaternionHBox = quaternionHBox;
        _spinBoxX = spinBoxX;
        _spinBoxY = spinBoxY;
        _spinBoxZ = spinBoxZ;
        _spinBoxW = spinBoxW;
    }

    public void SetValue(object value)
    {
        if (value is Quaternion quaternion)
        {
            _spinBoxX.Value = quaternion.X;
            _spinBoxY.Value = quaternion.Y;
            _spinBoxZ.Value = quaternion.Z;
            _spinBoxW.Value = quaternion.W;
        }
    }

    public Control Control => _quaternionHBox;

    public void SetEditable(bool editable)
    {
        _spinBoxX.Editable = editable;
        _spinBoxY.Editable = editable;
        _spinBoxZ.Editable = editable;
        _spinBoxW.Editable = editable;
    }
}
