using Godot;
using System;
using System.Collections.Generic;
using static Godot.Control;

namespace Visualize.Core;

public static partial class VisualControlTypes
{
    private static VisualControlInfo VisualArray(object initialValue, Type type, List<VisualSpinBox> debugExportSpinBoxes, Action<Array> valueChanged)
    {
        List<Control> controls = new();
        VBoxContainer arrayVBox = new() { SizeFlagsHorizontal = SizeFlags.ShrinkEnd | SizeFlags.Expand };
        Button addButton = new() { Text = "+" };

        Type elementType = type.GetElementType();
        Array array = initialValue as Array ?? Array.CreateInstance(elementType, 0);

        void AddNewEntryToArray()
        {
            Array newArray = Array.CreateInstance(elementType, array.Length + 1);
            Array.Copy(array, newArray, array.Length);
            array = newArray;
            valueChanged(array);

            object newValue = VisualMethods.CreateDefaultValue(elementType);
            int newIndex = array.Length - 1;

            VisualControlInfo control = CreateControlForType(newValue, elementType, debugExportSpinBoxes, v =>
            {
                array.SetValue(v, newIndex);
                valueChanged(array);
            });

            if (control.VisualControl != null)
            {
                Button minusButton = new() { Text = "-" };
                HBoxContainer hbox = new();

                minusButton.Pressed += () =>
                {
                    int indexToRemove = minusButton.GetParent().GetIndex();
                    arrayVBox.RemoveChild(hbox);
                    array = array.RemoveAt(indexToRemove);
                    valueChanged(array);
                };

                hbox.AddChild(control.VisualControl.Control);
                hbox.AddChild(minusButton);
                arrayVBox.AddChild(hbox);
                arrayVBox.MoveChild(addButton, arrayVBox.GetChildCount() - 1);
            }
        }

        for (int i = 0; i < array.Length; i++)
        {
            object value = array.GetValue(i);
            VisualControlInfo control = CreateControlForType(value, elementType, debugExportSpinBoxes, v =>
            {
                array.SetValue(v, i);
                valueChanged(array);
            });

            if (control.VisualControl != null)
            {
                SetControlValue(control.VisualControl.Control, value);

                Button minusButton = new() { Text = "-" };
                HBoxContainer hbox = new();

                minusButton.Pressed += () =>
                {
                    int indexToRemove = minusButton.GetParent().GetIndex();
                    arrayVBox.RemoveChild(hbox);
                    array = array.RemoveAt(indexToRemove);
                    valueChanged(array);
                };

                hbox.AddChild(control.VisualControl.Control);
                hbox.AddChild(minusButton);
                arrayVBox.AddChild(hbox);
            }
        }

        addButton.Pressed += AddNewEntryToArray;
        arrayVBox.AddChild(addButton);

        return new VisualControlInfo(new VBoxContainerControl(arrayVBox));
    }
}

public class VBoxContainerControl : IVisualControl
{
    private readonly VBoxContainer _vboxContainer;

    public VBoxContainerControl(VBoxContainer vboxContainer)
    {
        _vboxContainer = vboxContainer;
    }

    public void SetValue(object value)
    {
        // No specific value setting for VBoxContainer
    }

    public Control Control => _vboxContainer;

    public void SetEditable(bool editable)
    {
        // No specific editable setting for VBoxContainer
    }
}
