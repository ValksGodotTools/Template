using CSharpUtils;
using Godot;
using GodotUtils;
using System;
using System.Collections.Generic;
using System.Reflection;
using static Godot.Control;

namespace Template;

public static class VisualUIBuilder
{
    #region Method Parameters
    private static HBoxContainer CreateMethodParameterControls(MethodInfo method, List<DebugVisualSpinBox> debugExportSpinBoxes, object[] providedValues)
    {
        HBoxContainer hboxParams = new();

        ParameterInfo[] paramInfos = method.GetParameters();

        for (int i = 0; i < paramInfos.Length; i++)
        {
            ParameterInfo paramInfo = paramInfos[i];
            Type paramType = paramInfo.ParameterType;

            hboxParams.AddChild(new GLabel(paramInfo.Name.ToPascalCase().AddSpaceBeforeEachCapital()));

            //providedValues[i] ??= Activator.CreateInstance(paramType);

            int capturedIndex = i;

            Control control = CreateControlForType(null, null, paramType, debugExportSpinBoxes,
                v => providedValues[capturedIndex] = v);

            hboxParams.AddChild(control);
        }

        return hboxParams;
    }
    #endregion

    #region Control Types
    private static Control CreateControlForType(MemberInfo member, Node node, Type type, List<DebugVisualSpinBox> debugExportSpinBoxes, Action<object> valueChanged)
    {
        return type switch
        {
            // Handle numeric, enum and array types
            _ when type.IsNumericType() => CreateNumericControl(member, node, type, debugExportSpinBoxes, v => valueChanged(v)),
            _ when type.IsEnum => CreateEnumControl(member, node, type, v => valueChanged(v)),
            _ when type.IsArray => CreateArrayControl(member, node, type),

            // Handle C# specific types
            _ when type == typeof(bool) => CreateBoolControl(member, node, v => valueChanged(v)),
            _ when type == typeof(string) => CreateStringControl(member, node, v => valueChanged(v)),
            _ when type == typeof(object) => CreateObjectControl(member, node, v => valueChanged(v)),

            // Handle Godot specific types
            _ when type == typeof(Color) => CreateColorControl(member, node, v => valueChanged(v)),
            _ when type == typeof(Vector2) => CreateVector2Control(member, node, v => valueChanged(v)),
            _ when type == typeof(Vector2I) => CreateVector2IControl(member, node, v => valueChanged(v)),
            _ when type == typeof(Vector3) => CreateVector3Control(member, node, v => valueChanged(v)),
            _ when type == typeof(Vector3I) => CreateVector3IControl(member, node, v => valueChanged(v)),
            _ when type == typeof(Vector4) => CreateVector4Control(member, node, v => valueChanged(v)),
            _ when type == typeof(Vector4I) => CreateVector4IControl(member, node, v => valueChanged(v)),
            _ when type == typeof(Quaternion) => CreateQuaternionControl(member, node, v => valueChanged(v)),
            _ when type == typeof(NodePath) => CreateNodePathControl(member, node, v => valueChanged(v)),
            _ when type == typeof(StringName) => CreateStringNameControl(member, node, v => valueChanged(v)),

            // Handle unsupported types
            _ => throw new NotImplementedException($"The type '{type}' is not yet supported for the {nameof(VisualizeAttribute)}")
        };
    }

    private static Control CreateArrayControl(MemberInfo member, Node node, Type elementType)
    {
        VBoxContainer arrayVBox = new()
        {
            SizeFlagsHorizontal = SizeFlags.ShrinkEnd | SizeFlags.Expand
        };

        // Create initial LineEdit for each element in the array
        Array initialArray = VisualNodeHandler.GetMemberValue<Array>(member, node);
        object[] providedValues = new object[initialArray?.Length ?? 0];

        if (initialArray != null)
        {
            for (int i = 0; i < initialArray.Length; i++)
            {
                providedValues[i] = initialArray.GetValue(i);
                AddArrayEntry(arrayVBox, initialArray.GetValue(i), providedValues, i, elementType.GetElementType());
            }
        }

        // Add a button to add more entries
        Button addButton = new() { Text = "+" };

        addButton.Pressed += () =>
        {
            int newIndex = providedValues.Length;
            Array.Resize(ref providedValues, newIndex + 1);
            AddArrayEntry(arrayVBox, null, providedValues, newIndex, elementType.GetElementType());

            // Reorder the add button to always be at the bottom
            arrayVBox.MoveChild(addButton, arrayVBox.GetChildCount() - 1);
        };

        arrayVBox.AddChild(addButton);
        return arrayVBox;
    }

    private static Control CreateStringNameControl(MemberInfo member, Node node, Action<StringName> valueChanged)
    {
        StringName stringName = VisualNodeHandler.GetMemberValue<StringName>(member, node);
        string initialText = stringName != null ? stringName.ToString() : string.Empty;

        LineEdit lineEdit = new()
        {
            Text = initialText
        };

        lineEdit.TextChanged += text =>
        {
            valueChanged(new StringName(text));
        };

        return lineEdit;
    }

    private static Control CreateNodePathControl(MemberInfo member, Node node, Action<NodePath> valueChanged)
    {
        NodePath nodePath = VisualNodeHandler.GetMemberValue<NodePath>(member, node);

        string initialText = nodePath != null ? nodePath.ToString() : string.Empty;

        LineEdit lineEdit = new()
        {
            Text = initialText
        };

        lineEdit.TextChanged += text =>
        {
            valueChanged(new NodePath(text));
        };

        return lineEdit;
    }

    private static Control CreateObjectControl(MemberInfo member, Node node, Action<object> valueChanged)
    {
        LineEdit lineEdit = new()
        {
            Text = VisualNodeHandler.GetMemberValue<string>(member, node)
        };

        lineEdit.TextChanged += text =>
        {
            valueChanged(text);
        };

        return lineEdit;
    }

    private static Control CreateQuaternionControl(MemberInfo member, Node node, Action<Quaternion> valueChanged)
    {
        HBoxContainer quaternionHBox = new();

        Quaternion quaternion = VisualNodeHandler.GetMemberValue<Quaternion>(member, node);

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
            valueChanged(quaternion);
        };

        spinBoxY.ValueChanged += value =>
        {
            quaternion.Y = (float)value;
            valueChanged(quaternion);
        };

        spinBoxZ.ValueChanged += value =>
        {
            quaternion.Z = (float)value;
            valueChanged(quaternion);
        };

        spinBoxW.ValueChanged += value =>
        {
            quaternion.W = (float)value;
            valueChanged(quaternion);
        };

        quaternionHBox.AddChild(new GLabel("X"));
        quaternionHBox.AddChild(spinBoxX);
        quaternionHBox.AddChild(new GLabel("Y"));
        quaternionHBox.AddChild(spinBoxY);
        quaternionHBox.AddChild(new GLabel("Z"));
        quaternionHBox.AddChild(spinBoxZ);
        quaternionHBox.AddChild(new GLabel("W"));
        quaternionHBox.AddChild(spinBoxW);

        return quaternionHBox;
    }

    private static Control CreateVector2Control(MemberInfo member, Node node, Action<Vector2> valueChanged)
    {
        HBoxContainer vector2HBox = new();

        Vector2 vector2 = VisualNodeHandler.GetMemberValue<Vector2>(member, node);

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

        vector2HBox.AddChild(new GLabel("X"));
        vector2HBox.AddChild(spinBoxX);
        vector2HBox.AddChild(new GLabel("Y"));
        vector2HBox.AddChild(spinBoxY);

        return vector2HBox;
    }

    private static Control CreateVector2IControl(MemberInfo member, Node node, Action<Vector2I> valueChanged)
    {
        HBoxContainer vector2IHBox = new();

        Vector2I vector2I = VisualNodeHandler.GetMemberValue<Vector2I>(member, node);

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

        vector2IHBox.AddChild(new GLabel("X"));
        vector2IHBox.AddChild(spinBoxX);
        vector2IHBox.AddChild(new GLabel("Y"));
        vector2IHBox.AddChild(spinBoxY);

        return vector2IHBox;
    }

    private static Control CreateVector3Control(MemberInfo member, Node node, Action<Vector3> valueChanged)
    {
        HBoxContainer vector3HBox = new();

        Vector3 vector3 = VisualNodeHandler.GetMemberValue<Vector3>(member, node);

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

        vector3HBox.AddChild(new GLabel("X"));
        vector3HBox.AddChild(spinBoxX);
        vector3HBox.AddChild(new GLabel("Y"));
        vector3HBox.AddChild(spinBoxY);
        vector3HBox.AddChild(new GLabel("Z"));
        vector3HBox.AddChild(spinBoxZ);

        return vector3HBox;
    }

    private static Control CreateVector3IControl(MemberInfo member, Node node, Action<Vector3I> valueChanged)
    {
        HBoxContainer vector3IHBox = new();

        Vector3I vector3I = VisualNodeHandler.GetMemberValue<Vector3I>(member, node);

        SpinBox spinBoxX = CreateSpinBox(typeof(int));
        SpinBox spinBoxY = CreateSpinBox(typeof(int));
        SpinBox spinBoxZ = CreateSpinBox(typeof(int));

        spinBoxX.Value = vector3I.X;
        spinBoxY.Value = vector3I.Y;
        spinBoxZ.Value = vector3I.Z;

        spinBoxX.ValueChanged += value =>
        {
            vector3I.X = (int)value;
            valueChanged(vector3I);
        };

        spinBoxY.ValueChanged += value =>
        {
            vector3I.Y = (int)value;
            valueChanged(vector3I);
        };

        spinBoxZ.ValueChanged += value =>
        {
            vector3I.Z = (int)value;
            valueChanged(vector3I);
        };

        vector3IHBox.AddChild(new GLabel("X"));
        vector3IHBox.AddChild(spinBoxX);
        vector3IHBox.AddChild(new GLabel("Y"));
        vector3IHBox.AddChild(spinBoxY);
        vector3IHBox.AddChild(new GLabel("Z"));
        vector3IHBox.AddChild(spinBoxZ);

        return vector3IHBox;
    }

    private static Control CreateVector4Control(MemberInfo member, Node node, Action<Vector4> valueChanged)
    {
        HBoxContainer vector4HBox = new();

        Vector4 vector4 = VisualNodeHandler.GetMemberValue<Vector4>(member, node);

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

        vector4HBox.AddChild(new GLabel("X"));
        vector4HBox.AddChild(spinBoxX);
        vector4HBox.AddChild(new GLabel("Y"));
        vector4HBox.AddChild(spinBoxY);
        vector4HBox.AddChild(new GLabel("Z"));
        vector4HBox.AddChild(spinBoxZ);
        vector4HBox.AddChild(new GLabel("W"));
        vector4HBox.AddChild(spinBoxW);

        return vector4HBox;
    }

    private static Control CreateVector4IControl(MemberInfo member, Node node, Action<Vector4I> valueChanged)
    {
        HBoxContainer vector4IHBox = new();

        Vector4I vector4I = VisualNodeHandler.GetMemberValue<Vector4I>(member, node);

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

        vector4IHBox.AddChild(new GLabel("X"));
        vector4IHBox.AddChild(spinBoxX);
        vector4IHBox.AddChild(new GLabel("Y"));
        vector4IHBox.AddChild(spinBoxY);
        vector4IHBox.AddChild(new GLabel("Z"));
        vector4IHBox.AddChild(spinBoxZ);
        vector4IHBox.AddChild(new GLabel("W"));
        vector4IHBox.AddChild(spinBoxW);

        return vector4IHBox;
    }

    private static Control CreateNumericControl(MemberInfo member, Node node, Type type, List<DebugVisualSpinBox> debugExportSpinBoxes, Action<double> valueChanged)
    {
        SpinBox spinBox = CreateSpinBox(type);

        double value = VisualNodeHandler.GetMemberValue<double>(member, node);

        spinBox.Value = value;
        spinBox.ValueChanged += value => valueChanged(value);

        return spinBox;
    }

    private static Control CreateBoolControl(MemberInfo member, Node node, Action<bool> valueChanged)
    {
        CheckBox checkBox = new()
        {
            ButtonPressed = VisualNodeHandler.GetMemberValue<bool>(member, node)
        };
        checkBox.Toggled += value => valueChanged(value);

        return checkBox;
    }

    private static Control CreateColorControl(MemberInfo member, Node node, Action<Color> valueChanged)
    {
        GColorPickerButton colorPickerButton = new(VisualNodeHandler.GetMemberValue<Color>(member, node));
        colorPickerButton.OnColorChanged += color => valueChanged(color);

        return colorPickerButton.Control;
    }

    private static Control CreateStringControl(MemberInfo member, Node node, Action<string> valueChanged)
    {
        LineEdit lineEdit = new()
        {
            Text = VisualNodeHandler.GetMemberValue<string>(member, node)
        };

        lineEdit.TextChanged += text => valueChanged(text);

        return lineEdit;
    }

    private static Control CreateEnumControl(MemberInfo member, Node node, Type type, Action<object> valueChanged)
    {
        GOptionButtonEnum optionButton = new(type);
        optionButton.Select(VisualNodeHandler.GetMemberValue(member, node));
        optionButton.OnItemSelected += item => valueChanged(item);

        return optionButton.Control;
    }
    #endregion

    #region Specific Util Functions
    public static void CreateVisualPanels(List<DebugVisualNode> debugVisualNodes, List<DebugVisualSpinBox> debugExportSpinBoxes)
    {
        foreach (DebugVisualNode debugVisualNode in debugVisualNodes)
        {
            Node node = debugVisualNode.Node;

            VBoxContainer vbox = CreateVisualContainer(node.Name);

            AddMemberInfoElements(vbox, debugVisualNode.Properties, node, debugExportSpinBoxes);

            AddMemberInfoElements(vbox, debugVisualNode.Fields, node, debugExportSpinBoxes);

            AddMethodInfoElements(vbox, debugVisualNode.Methods, node, debugExportSpinBoxes);

            // All debug UI elements should not be influenced by the game world environments lighting
            vbox.GetChildren<Control>().ForEach(child => child.SetUnshaded());

            node.AddChild(vbox);

            const float INFO_PANEL_SCALE_FACTOR = 0.6f;

            vbox.Scale = Vector2.One * INFO_PANEL_SCALE_FACTOR;

            if (debugVisualNode.InitialPosition != Vector2.Zero)
            {
                vbox.GlobalPosition = debugVisualNode.InitialPosition;
            }
        }
    }

    private static VBoxContainer CreateVisualContainer(string nodeName)
    {
        VBoxContainer vbox = new()
        {
            // Ensure this info is rendered above all game elements
            ZIndex = (int)RenderingServer.CanvasItemZMax
        };

        GLabel label = new(nodeName);

        vbox.AddChild(label);

        return vbox;
    }

    private static void AddArrayEntry(VBoxContainer arrayVBox, object initialValue, object[] providedValues, int index, Type elementType)
    {
        HBoxContainer entryHBox = new();

        LineEdit lineEdit = new();
        lineEdit.Alignment = HorizontalAlignment.Center;
        lineEdit.Text = initialValue?.ToString() ?? string.Empty;
        lineEdit.TextChanged += text => UpdateArrayValue(arrayVBox, providedValues, index, elementType);

        GButton removeButton = new("-");
        removeButton.CustomMinimumSize = Vector2.One * 30;
        removeButton.SetUnshaded();

        removeButton.Pressed += () =>
        {
            arrayVBox.RemoveChild(entryHBox);
            UpdateArrayValue(arrayVBox, providedValues, index, elementType);
        };

        entryHBox.AddChild(lineEdit);
        entryHBox.AddChild(removeButton);
        arrayVBox.AddChild(entryHBox);
    }

    private static void UpdateArrayValue(VBoxContainer arrayVBox, object[] providedValues, int index, Type elementType)
    {
        List<object> elements = [];

        foreach (HBoxContainer entryHBox in arrayVBox.GetChildren<HBoxContainer>())
        {
            LineEdit lineEdit = entryHBox.GetChild<LineEdit>(0);

            string text = lineEdit.Text.Trim();

            if (!string.IsNullOrEmpty(text))
            {
                try
                {
                    elements.Add(Convert.ChangeType(text, elementType));
                }
                catch (Exception ex)
                {
                    GD.Print($"Failed to convert value '{text}' to type '{elementType}': {ex.Message}");

                    lineEdit.Text = "";
                }
            }
        }

        Array array = Array.CreateInstance(elementType, elements.Count);

        for (int i = 0; i < elements.Count; i++)
        {
            array.SetValue(elements[i], i);
        }

        providedValues[index] = array;
    }

    private static object ConvertNumericValue(SpinBox spinBox, double value, Type paramType)
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
                string errorMessage = $"The provided value '{value}' is not assignable to the parameter type '{paramType}'.";
                throw new InvalidOperationException(errorMessage);
            }
        }

        return convertedValue;
    }

    private static Button CreateMethodButton(MethodInfo method, Node node, ParameterInfo[] paramInfos, object[] providedValues)
    {
        Button button = new()
        {
            Text = method.Name,
            SizeFlagsHorizontal = SizeFlags.ShrinkCenter
        };

        button.Pressed += () =>
        {
            object[] parameters = ParameterConverter.ConvertParameterInfoToObjectArray(paramInfos, providedValues);

            method.Invoke(node, parameters);
        };

        return button;
    }

    private static void AddMethodInfoElements(VBoxContainer vbox, IEnumerable<MethodInfo> methods, Node node, List<DebugVisualSpinBox> debugExportSpinBoxes)
    {
        foreach (MethodInfo method in methods)
        {
            if (method.DeclaringType.IsSubclassOf(typeof(GodotObject)))
            {
                ParameterInfo[] paramInfos = method.GetParameters();
                object[] providedValues = new object[paramInfos.Length];

                HBoxContainer hboxParams = CreateMethodParameterControls(method, debugExportSpinBoxes, providedValues);

                vbox.AddChild(hboxParams);

                Button button = CreateMethodButton(method, node, paramInfos, providedValues);

                vbox.AddChild(button);
            }
        }
    }

    private static void AddMemberInfoElements(VBoxContainer vbox, IEnumerable<MemberInfo> members, Node node, List<DebugVisualSpinBox> debugExportSpinBoxes)
    {
        foreach (MemberInfo member in members)
        {
            Control element = CreateMemberInfoElement(member, node, debugExportSpinBoxes);
            vbox.AddChild(element);
        }
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

        Type type = VisualNodeHandler.GetMemberType(member);

        Control element = CreateControlForType(member, node, type, debugExportSpinBoxes, v =>
        {
            VisualNodeHandler.SetMemberValue(member, node, v);
        });

        hbox.AddChild(element);

        return hbox;
    }

    private static SpinBox CreateSpinBox(Type type)
    {
        SpinBox spinBox = new()
        {
            UpdateOnTextChanged = true,
            AllowLesser = false,
            AllowGreater = false,
            MinValue = int.MinValue,
            MaxValue = int.MaxValue,
            Alignment = HorizontalAlignment.Center
        };

        spinBox.Step = type switch
        {
            _ when type == typeof(float) => 0.1,
            _ when type == typeof(double) => 0.1,
            _ when type == typeof(decimal) => 0.01,
            _ when type == typeof(int) => 1,
            _ => 1
        };

        return spinBox;
    }
    #endregion
}
