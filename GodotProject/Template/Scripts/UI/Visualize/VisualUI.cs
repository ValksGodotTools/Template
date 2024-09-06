using CSharpUtils;
using Godot;
using GodotUtils;
using System;
using System.Collections.Generic;
using System.Reflection;
using static Godot.Control;

namespace Template;

public static class VisualUI
{
    private const float INFO_PANEL_SCALE_FACTOR = 0.6f;

    public static void CreateVisualUIs(List<DebugVisualNode> debugVisualNodes, List<DebugVisualSpinBox> debugExportSpinBoxes)
    {
        foreach (DebugVisualNode debugVisualNode in debugVisualNodes)
        {
            Node node = debugVisualNode.Node;

            VBoxContainer vbox = new()
            {
                // Ensure this info is rendered above all game elements
                ZIndex = (int)RenderingServer.CanvasItemZMax
            };

            GLabel label = new(node.Name);

            vbox.AddChild(label);

            foreach (PropertyInfo property in debugVisualNode.Properties)
            {
                object value = property.GetValue(node);

                Control element = CreateMemberInfoElement(property, node, debugExportSpinBoxes);
                vbox.AddChild(element);
            }

            foreach (FieldInfo field in debugVisualNode.Fields)
            {
                object value = field.GetValue(node);

                Control element = CreateMemberInfoElement(field, node, debugExportSpinBoxes);
                vbox.AddChild(element);
            }

            foreach (MethodInfo method in debugVisualNode.Methods)
            {
                if (method.DeclaringType.IsSubclassOf(typeof(GodotObject)))
                {
                    ParameterInfo[] paramInfos = method.GetParameters();

                    HBoxContainer hboxParams = new();

                    object[] providedValues = new object[paramInfos.Length];

                    for (int i = 0; i < paramInfos.Length; i++)
                    {
                        ParameterInfo paramInfo = paramInfos[i];
                        Type paramType = paramInfo.ParameterType;

                        hboxParams.AddChild(new GLabel(paramInfo.Name.ToPascalCase().AddSpaceBeforeEachCapital()));

                        int index = i; // Capture the current value of i

                        if (paramType.IsNumericType())
                        {
                            SpinBox spinBox = SpinBox(debugExportSpinBoxes, paramType);

                            spinBox.ValueChanged += value =>
                            {
                                object convertedValue = value;

                                try
                                {
                                    convertedValue = Convert.ChangeType(value, paramType);
                                }
                                catch
                                {
                                    (object Min, object Max) = TypeRangeConstraints.GetRange(paramType);

                                    if (Convert.ToDouble(value) < Convert.ToDouble(Min))
                                    {
                                        spinBox.Value = Convert.ToDouble(Min);
                                        convertedValue = Min;
                                    }
                                    else if (Convert.ToDouble(value) > Convert.ToDouble(Max))
                                    {
                                        spinBox.Value = Convert.ToDouble(Max);
                                        convertedValue = Max;
                                    }
                                    else
                                    {
                                        string errorMessage = $"The provided value '{value}' for parameter '{paramInfo.Name}' is not assignable to the parameter type '{paramType}'.";

                                        throw new InvalidOperationException(errorMessage);
                                    }
                                }

                                providedValues[index] = convertedValue; // Use the captured index
                            };

                            hboxParams.AddChild(spinBox);
                        }
                        else if (paramType == typeof(string))
                        {
                            LineEdit lineEdit = new();

                            lineEdit.TextChanged += text =>
                            {
                                providedValues[index] = text;
                            };

                            hboxParams.AddChild(lineEdit);
                        }
                        else if (paramType.IsEnum)
                        {
                            GOptionButtonEnum optionButton = new(paramType);

                            optionButton.OnItemSelected += item =>
                            {
                                providedValues[index] = item;
                            };

                            hboxParams.AddChild(optionButton.Control);
                        }
                        else if (paramType == typeof(bool))
                        {
                            CheckBox checkBox = new();

                            checkBox.Toggled += value =>
                            {
                                providedValues[index] = value;
                                checkBox.ReleaseFocus();
                            };

                            hboxParams.AddChild(checkBox);
                        }
                        else if (paramType == typeof(Godot.Color))
                        {
                            GColorPickerButton colorPickerButton = new();

                            colorPickerButton.OnColorChanged += color =>
                            {
                                providedValues[index] = color;
                            };

                            hboxParams.AddChild(colorPickerButton.Control);
                        }
                    }

                    vbox.AddChild(hboxParams);

                    Button button = new()
                    {
                        Text = method.Name,
                        SizeFlagsHorizontal = SizeFlags.ShrinkCenter
                    };

                    button.Pressed += () =>
                    {
                        object[] parameters = ParameterConverter
                            .ConvertParameterInfoToObjectArray(paramInfos, providedValues);

                        method.Invoke(node, parameters);
                    };

                    vbox.AddChild(button);
                }
            }

            // All debug UI elements should not be influenced by the game world environments lighting
            vbox.GetChildren<Control>().ForEach(child => child.SetUnshaded());

            node.AddChild(vbox);

            vbox.Scale = Vector2.One * INFO_PANEL_SCALE_FACTOR;

            if (debugVisualNode.InitialPosition != Vector2.Zero)
            {
                vbox.GlobalPosition = debugVisualNode.InitialPosition;
            }
        }
    }

    public static void CreateStepPrecisionUI(List<DebugVisualSpinBox> debugExportSpinBoxes, VBoxContainer controlPanel, SceneTree tree)
    {
        HBoxContainer hbox = new();

        Label label = new()
        {
            Text = "Step Precision",
            SizeFlagsHorizontal = SizeFlags.ExpandFill
        };

        hbox.AddChild(label);
        hbox.AddChild(CreateStepPrecisionOptionButton(debugExportSpinBoxes));

        controlPanel.AddChild(hbox);

        GButton unfocus = new("Unfocus Active Element");
        unfocus.Pressed += tree.UnfocusCurrentControl;
        controlPanel.AddChild(unfocus);
    }

    private static HBoxContainer CreateMemberInfoElement(MemberInfo member, Node node, List<DebugVisualSpinBox> debugExportSpinBoxes)
    {
        HBoxContainer hbox = new();

        GLabel label = new()
        {
            Text = member.Name.ToPascalCase().AddSpaceBeforeEachCapital(),
            SizeFlagsHorizontal = SizeFlags.ExpandFill
        };

        hbox.AddChild(label);

        Control element;

        Type type = VisualNodeHandler.GetMemberType(member);

        if (type.IsNumericType())
        {
            SpinBox spinBox = SpinBox(debugExportSpinBoxes, type);

            double value = VisualNodeHandler.GetMemberValue<double>(member, node);

            Type valueType = value.GetType();

            double step = 0.1;

            if (valueType.IsWholeNumber())
            {
                step = 1;
            }

            spinBox.Step = step;
            spinBox.Value = value;

            spinBox.ValueChanged += value =>
            {
                VisualNodeHandler.SetMemberValue(member, node, value);
            };

            element = spinBox;
        }
        else if (type == typeof(bool))
        {
            CheckBox checkBox = new();

            checkBox.ButtonPressed = VisualNodeHandler.GetMemberValue<bool>(member, node);

            checkBox.Toggled += value =>
            {
                VisualNodeHandler.SetMemberValue(member, node, value);
            };

            element = checkBox;
        }
        else if (type == typeof(Godot.Color))
        {
            GColorPickerButton colorPickerButton = new(VisualNodeHandler.GetMemberValue<Color>(member, node));

            colorPickerButton.OnColorChanged += color =>
            {
                VisualNodeHandler.SetMemberValue(member, node, color);
            };

            element = colorPickerButton.Control;
        }
        else if (type == typeof(string))
        {
            LineEdit lineEdit = new();

            lineEdit.Text = VisualNodeHandler.GetMemberValue<string>(member, node);

            lineEdit.TextChanged += text =>
            {
                VisualNodeHandler.SetMemberValue(member, node, text);
            };

            element = lineEdit;
        }
        else if (type.IsEnum)
        {
            GOptionButtonEnum optionButton = new(VisualNodeHandler.GetMemberType(member));
            optionButton.Select(VisualNodeHandler.GetMemberValue(member, node));

            optionButton.OnItemSelected += item =>
            {
                VisualNodeHandler.SetMemberValue(member, node, item);
            };

            element = optionButton.Control;
        }
        else
        {
            throw new NotImplementedException($"The type '{type}' is not yet supported for the {nameof(VisualizeAttribute)}");
        }

        hbox.AddChild(element);

        return hbox;
    }

    private static SpinBox SpinBox(List<DebugVisualSpinBox> debugExportSpinBoxes, Type type)
    {
        SpinBox spinBox = new()
        {
            UpdateOnTextChanged = true,
            AllowLesser = true,
            AllowGreater = true,
            Alignment = HorizontalAlignment.Center
        };

        debugExportSpinBoxes.Add(new DebugVisualSpinBox
        {
            SpinBox = spinBox,
            Type = type
        });

        return spinBox;
    }

    private static OptionButton CreateStepPrecisionOptionButton(List<DebugVisualSpinBox> debugExportSpinBoxes)
    {
        GOptionButton optionButton = new("10", "1", "0.1", "0.01", "0.001");

        // Default precision will be 0.1
        optionButton.Select(2);

        optionButton.OnItemSelected += itemId =>
        {
            foreach (DebugVisualSpinBox debugExportSpinBox in debugExportSpinBoxes)
            {
                double precision = Convert.ToDouble(optionButton.Control.GetItemText(itemId));

                // Whole Numbers (non-decimals)
                if (debugExportSpinBox.Type.IsWholeNumber())
                {
                    // Round the precision to the nearest integer
                    double rounded = Mathf.RoundToInt(precision);

                    // Ensure rounded is never zero, if it is, change rounded to 1
                    rounded = rounded == 0 ? 1 : rounded;

                    debugExportSpinBox.SpinBox.Step = rounded;
                }
                else
                // Decimal numbers
                {
                    debugExportSpinBox.SpinBox.Step = precision;
                }
            }
        };

        return optionButton.Control;
    }
}
