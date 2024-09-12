using Godot;
using System;

namespace Visualize.Core;

public static partial class VisualControlTypes
{
    private static VisualControlInfo VisualVector2I(object initialValue, Action<Vector2I> valueChanged)
    {
        HBoxContainer vector2IHBox = new();

        Vector2I vector2I = (Vector2I)initialValue;

        SpinBox spinBoxX = CreateSpinBox(typeof(int));
        SpinBox spinBoxY = CreateSpinBox(typeof(int));

        spinBoxX.Value = vector2I.X;
        spinBoxY.Value = vector2I.Y;

        spinBoxX.ValueChanged += value =>
        {
            vector2I.X = (int)value;
            valueChanged(vector2I);
        };

        spinBoxY.ValueChanged += value =>
        {
            vector2I.Y = (int)value;
            valueChanged(vector2I);
        };

        vector2IHBox.AddChild(new Label { Text = "X" });
        vector2IHBox.AddChild(spinBoxX);
        vector2IHBox.AddChild(new Label { Text = "Y" });
        vector2IHBox.AddChild(spinBoxY);

        return new VisualControlInfo(new Vector2IControl(vector2IHBox, spinBoxX, spinBoxY));
    }

    public class Vector2IControl : IVisualControl
    {
        private readonly HBoxContainer _vector2IHBox;
        private readonly SpinBox _spinBoxX;
        private readonly SpinBox _spinBoxY;

        public Vector2IControl(HBoxContainer vector2IHBox, SpinBox spinBoxX, SpinBox spinBoxY)
        {
            _vector2IHBox = vector2IHBox;
            _spinBoxX = spinBoxX;
            _spinBoxY = spinBoxY;
        }

        public void SetValue(object value)
        {
            if (value is Vector2I vector2I)
            {
                _spinBoxX.Value = vector2I.X;
                _spinBoxY.Value = vector2I.Y;
            }
        }

        public Control Control => _vector2IHBox;

        public void SetEditable(bool editable)
        {
            _spinBoxX.Editable = editable;
            _spinBoxY.Editable = editable;
        }
    }
}
