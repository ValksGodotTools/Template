using Godot;
using System;

namespace Visualize.Core;

public static partial class VisualControlTypes
{
    private static VisualControlInfo VisualVector2(object initialValue, Action<Vector2> valueChanged)
    {
        HBoxContainer vector2HBox = new();

        Vector2 vector2 = (Vector2)initialValue;

        SpinBox spinBoxX = CreateSpinBox(typeof(float));
        SpinBox spinBoxY = CreateSpinBox(typeof(float));

        spinBoxX.Value = vector2.X;
        spinBoxY.Value = vector2.Y;

        spinBoxX.ValueChanged += value =>
        {
            vector2.X = (float)value;
            valueChanged(vector2);
        };

        spinBoxY.ValueChanged += value =>
        {
            vector2.Y = (float)value;
            valueChanged(vector2);
        };

        vector2HBox.AddChild(new Label { Text = "X" });
        vector2HBox.AddChild(spinBoxX);
        vector2HBox.AddChild(new Label { Text = "Y" });
        vector2HBox.AddChild(spinBoxY);

        return new VisualControlInfo(new Vector2Control(vector2HBox, spinBoxX, spinBoxY));
    }

    public class Vector2Control : IVisualControl
    {
        private readonly HBoxContainer _vector2HBox;
        private readonly SpinBox _spinBoxX;
        private readonly SpinBox _spinBoxY;

        public Vector2Control(HBoxContainer vector2HBox, SpinBox spinBoxX, SpinBox spinBoxY)
        {
            _vector2HBox = vector2HBox;
            _spinBoxX = spinBoxX;
            _spinBoxY = spinBoxY;
        }

        public void SetValue(object value)
        {
            if (value is Vector2 vector2)
            {
                _spinBoxX.Value = vector2.X;
                _spinBoxY.Value = vector2.Y;
            }
        }

        public Control Control => _vector2HBox;

        public void SetEditable(bool editable)
        {
            _spinBoxX.Editable = editable;
            _spinBoxY.Editable = editable;
        }
    }
}
