using CSharpUtils;
using Godot;
using GodotUtils;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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

            providedValues[i] = CreateDefaultValue(paramType);

            int capturedIndex = i;

            Control control = CreateControlForType(providedValues[i], paramType, debugExportSpinBoxes,
                v => providedValues[capturedIndex] = v);

            if (control != null)
            {
                hboxParams.AddChild(new GLabel(paramInfo.Name.ToPascalCase().AddSpaceBeforeEachCapital()));
                hboxParams.AddChild(control);
            }
        }

        return hboxParams;
    }
    #endregion

    #region Control Types
    private static Control CreateControlForType(object initialValue, Type type, List<DebugVisualSpinBox> debugExportSpinBoxes, Action<object> valueChanged)
    {
        Control control = type switch
        {
            // Handle numeric, enum and array types
            _ when type.IsNumericType() => CreateNumericControl(initialValue, type, debugExportSpinBoxes, v => valueChanged(v)),
            _ when type.IsEnum => CreateEnumControl(initialValue, type, v => valueChanged(v)),
            _ when type.IsArray => CreateArrayControl(initialValue, type, debugExportSpinBoxes, v => valueChanged(v)),
            _ when type.IsClass && !type.IsSubclassOf(typeof(GodotObject)) => CreateClassControl(initialValue, type, debugExportSpinBoxes, v => valueChanged(v)),
            _ when type.IsGenericType && type.GetGenericTypeDefinition() == typeof(List<>) => CreateListControl(initialValue, type, debugExportSpinBoxes, v => valueChanged(v)),
            _ when type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Dictionary<,>) => CreateDictionaryControl(initialValue, type, debugExportSpinBoxes, v => valueChanged(v)),

            // Handle C# specific types
            _ when type == typeof(bool) => CreateBoolControl(initialValue, v => valueChanged(v)),
            _ when type == typeof(string) => CreateStringControl(initialValue, v => valueChanged(v)),
            _ when type == typeof(object) => CreateObjectControl(initialValue, v => valueChanged(v)),

            // Handle Godot specific types
            _ when type == typeof(Color) => CreateColorControl(initialValue, v => valueChanged(v)),
            _ when type == typeof(Vector2) => CreateVector2Control(initialValue, v => valueChanged(v)),
            _ when type == typeof(Vector2I) => CreateVector2IControl(initialValue, v => valueChanged(v)),
            _ when type == typeof(Vector3) => CreateVector3Control(initialValue, v => valueChanged(v)),
            _ when type == typeof(Vector3I) => CreateVector3IControl(initialValue, v => valueChanged(v)),
            _ when type == typeof(Vector4) => CreateVector4Control(initialValue, v => valueChanged(v)),
            _ when type == typeof(Vector4I) => CreateVector4IControl(initialValue, v => valueChanged(v)),
            _ when type == typeof(Quaternion) => CreateQuaternionControl(initialValue, v => valueChanged(v)),
            _ when type == typeof(NodePath) => CreateNodePathControl(initialValue, v => valueChanged(v)),
            _ when type == typeof(StringName) => CreateStringNameControl(initialValue, v => valueChanged(v)),

            _ => null
        };

        /*if (control == null)
        {
            GD.Print($"The type '{type}' is not supported for {nameof(VisualizeAttribute)}");
        }*/

        return control;
    }

    private static Control CreateClassControl(object target, Type type, List<DebugVisualSpinBox> debugExportSpinBoxes, Action<object> valueChanged)
    {
        BindingFlags flags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static;

        PropertyInfo[] properties = type.GetProperties(flags);
        FieldInfo[] fields = type.GetFields(flags);
        MethodInfo[] methods = type.GetMethods(flags | BindingFlags.DeclaredOnly);

        VBoxContainer vbox = new();

        foreach (PropertyInfo property in properties)
        {
            object initialValue = property.GetValue(target); // Non-Static method requires a target

            Control control = CreateControlForType(initialValue, property.PropertyType, debugExportSpinBoxes, v =>
            {
                property.SetValue(target, v);
                valueChanged(target);
            });

            if (control != null)
            {
                vbox.AddChild(control);
            }
        }

        foreach (FieldInfo field in fields)
        {
            object initialValue = field.GetValue(target);

            Control control = CreateControlForType(initialValue, field.FieldType, debugExportSpinBoxes, v =>
            {
                field.SetValue(target, v);
                valueChanged(target);
            });

            if (control != null)
            {
                vbox.AddChild(control);
            }
        }

        foreach (MethodInfo method in methods)
        {
            ParameterInfo[] paramInfos = method.GetParameters();
            object[] providedValues = new object[paramInfos.Length];

            HBoxContainer hboxParams = CreateMethodParameterControls(method, debugExportSpinBoxes, providedValues);

            vbox.AddChild(hboxParams);

            Button button = CreateMethodButton(method, target, paramInfos, providedValues);

            vbox.AddChild(button);
        }

        return vbox;
    }

    private static Control CreateDictionaryControl(object initialValue, Type type, List<DebugVisualSpinBox> debugExportSpinBoxes, Action<IDictionary> valueChanged)
    {
        VBoxContainer dictionaryVBox = new()
        {
            SizeFlagsHorizontal = SizeFlags.ShrinkEnd | SizeFlags.Expand
        };

        Button addButton = new() { Text = "+" };

        Type[] genericArguments = type.GetGenericArguments();

        Type keyType = genericArguments[0];
        Type valueType = genericArguments[1];

        object defaultKey = CreateDefaultValue(keyType);
        object defaultValue = CreateDefaultValue(valueType);

        IDictionary dictionary = initialValue as IDictionary ?? (IDictionary)Activator.CreateInstance(typeof(Dictionary<,>).MakeGenericType(keyType, valueType));

        // Add the initial values to the UI
        foreach (DictionaryEntry entry in dictionary)
        {
            object key = entry.Key;
            object value = entry.Value;

            // The control for changing the value of the dictionary
            Control valueControl = CreateControlForType(value, valueType, debugExportSpinBoxes, v =>
            {
                dictionary[key] = v;
                valueChanged(dictionary);
            });

            // The control for changing the key of the dictionary
            Control keyControl = CreateControlForType(key, keyType, debugExportSpinBoxes, v =>
            {
                if (dictionary.Contains(v))
                {
                    // This key exists already, do nothing
                }
                else
                {
                    if (v.GetType() != keyType)
                    {
                        throw new ArgumentException($"Type mismatch: Expected {keyType}, got {v.GetType()}");
                    }
                    else
                    {
                        // This key does not exist, remove the old key and add the new key
                        dictionary.Remove(key);
                        dictionary[v] = value;

                        // Update key with the new key
                        key = v;

                        valueChanged(dictionary);

                        // Visually reset the value for this dictionary back to the default value
                        SetControlValue(valueControl, defaultValue);
                    }
                }
            });

            if (keyControl != null && valueControl != null)
            {
                SetControlValue(keyControl, key);
                SetControlValue(valueControl, value);

                GButton removeKeyEntryButton = new("-");

                HBoxContainer hbox = new();

                removeKeyEntryButton.Pressed += () =>
                {
                    dictionaryVBox.RemoveChild(hbox);
                    dictionary.Remove(key);
                    valueChanged(dictionary);
                };

                hbox.AddChild(keyControl);
                hbox.AddChild(valueControl);
                hbox.AddChild(removeKeyEntryButton);

                dictionaryVBox.AddChild(hbox);
            }
        }

        void AddNewEntryToDictionary()
        {
            // Check if the defaultKey exists in the dictionary, if it does not then add it with
            // the default value
            if (!dictionary.Contains(defaultKey))
            {
                dictionary[defaultKey] = defaultValue;
                valueChanged(dictionary);
            }

            // Keep track of the old key
            object oldKey = defaultKey;

            // The control for changing the value of the dictionary
            Control valueControl = CreateControlForType(defaultValue, valueType, debugExportSpinBoxes, v =>
            {
                dictionary[oldKey] = v;
                valueChanged(dictionary);
            });

            // The control for changing the key of the dictionary
            Control keyControl = CreateControlForType(defaultKey, keyType, debugExportSpinBoxes, v =>
            {
                if (dictionary.Contains(v))
                {
                    // This key exists already, do nothing
                }
                else
                {
                    if (v.GetType() != keyType)
                    {
                        throw new ArgumentException($"Type mismatch: Expected {keyType}, got {v.GetType()}");
                    }
                    else
                    {
                        // This key does not exist, remove the old key and add the new key
                        dictionary.Remove(oldKey);
                        dictionary[v] = defaultValue;

                        // Update old key with the new key
                        oldKey = v;

                        valueChanged(dictionary);

                        // Visually reset the value for this dictionary back to the default value
                        SetControlValue(valueControl, defaultValue);
                    }
                }
            });

            if (keyControl != null && valueControl != null)
            {
                GButton removeKeyEntryButton = new("-");

                HBoxContainer hbox = new();

                removeKeyEntryButton.Pressed += () =>
                {
                    dictionaryVBox.RemoveChild(hbox);
                    dictionary.Remove(oldKey);
                    valueChanged(dictionary);
                };

                hbox.AddChild(keyControl);
                hbox.AddChild(valueControl);
                hbox.AddChild(removeKeyEntryButton);

                dictionaryVBox.AddChild(hbox);

                // Reorder the add button to always be at the bottom
                dictionaryVBox.MoveChild(addButton, dictionaryVBox.GetChildCount() - 1);
            }
        }

        // Add a button to add more entries
        addButton.Pressed += AddNewEntryToDictionary;
        dictionaryVBox.AddChild(addButton);

        return dictionaryVBox;
    }

    private static Control CreateListControl(object initialValue, Type type, List<DebugVisualSpinBox> debugExportSpinBoxes, Action<IList> valueChanged)
    {
        VBoxContainer listVBox = new()
        {
            SizeFlagsHorizontal = SizeFlags.ShrinkEnd | SizeFlags.Expand
        };

        Button addButton = new() { Text = "+" };

        Type elementType = type.GetGenericArguments()[0];
        IList list = initialValue as IList ?? (IList)Activator.CreateInstance(typeof(List<>).MakeGenericType(elementType));

        void AddNewEntryToList()
        {
            // Create a new instance of the list element type
            object newValue = CreateDefaultValue(elementType);
            list.Add(newValue);
            valueChanged(list);

            int newIndex = list.Count - 1;

            Control control = CreateControlForType(newValue, elementType, debugExportSpinBoxes, v =>
            {
                list[newIndex] = v;
                valueChanged(list);
            });

            if (control != null)
            {
                // Add a new entry to the UI
                HBoxContainer hbox = new();

                Button minusButton = new() { Text = "-" };
                minusButton.Pressed += () =>
                {
                    int indexToRemove = minusButton.GetParent().GetIndex();
                    listVBox.RemoveChild(hbox);
                    list.RemoveAt(indexToRemove);
                    valueChanged(list);
                };

                hbox.AddChild(control);
                hbox.AddChild(minusButton);
                listVBox.AddChild(hbox);

                // Reorder the add button to always be at the bottom
                listVBox.MoveChild(addButton, listVBox.GetChildCount() - 1);
            }
        }

        // Initialize the UI with the existing list elements
        for (int i = 0; i < list.Count; i++)
        {
            object value = list[i];

            Control control = CreateControlForType(value, elementType, debugExportSpinBoxes, v =>
            {
                list[i] = v;
                valueChanged(list);
            });

            if (control != null)
            {
                SetControlValue(control, value);

                Button minusButton = new() { Text = "-" };

                HBoxContainer hbox = new();

                minusButton.Pressed += () =>
                {
                    int indexToRemove = minusButton.GetParent().GetIndex();
                    listVBox.RemoveChild(hbox);
                    list.RemoveAt(indexToRemove);
                    valueChanged(list);
                };

                hbox.AddChild(control);
                hbox.AddChild(minusButton);
                listVBox.AddChild(hbox);
            }
        }

        // Add a button to add more entries
        addButton.Pressed += AddNewEntryToList;
        listVBox.AddChild(addButton);

        return listVBox;
    }

    private static Control CreateArrayControl(object initialValue, Type type, List<DebugVisualSpinBox> debugExportSpinBoxes, Action<Array> valueChanged)
    {
        VBoxContainer arrayVBox = new()
        {
            SizeFlagsHorizontal = SizeFlags.ShrinkEnd | SizeFlags.Expand
        };

        Button addButton = new() { Text = "+" };

        Type elementType = type.GetElementType();
        Array array = initialValue as Array ?? Array.CreateInstance(elementType, 0);

        void AddNewEntryToArray()
        {
            // Create a new array with the updated length
            Array newArray = Array.CreateInstance(elementType, array.Length + 1);
            Array.Copy(array, newArray, array.Length);
            array = newArray;
            valueChanged(array);

            object newValue = CreateDefaultValue(elementType);

            int newIndex = array.Length - 1;

            Control control = CreateControlForType(newValue, elementType, debugExportSpinBoxes, v =>
            {
                array.SetValue(v, newIndex);

                valueChanged(array);
            });

            if (control != null)
            {
                Button minusButton = new() { Text = "-" };

                // Add a new entry to the UI
                HBoxContainer hbox = new();

                minusButton.Pressed += () =>
                {
                    int indexToRemove = minusButton.GetParent().GetIndex();
                    arrayVBox.RemoveChild(hbox);
                    array = array.RemoveAt(indexToRemove);
                    valueChanged(array);
                };

                hbox.AddChild(control);
                hbox.AddChild(minusButton);
                arrayVBox.AddChild(hbox);

                // Reorder the add button to always be at the bottom
                arrayVBox.MoveChild(addButton, arrayVBox.GetChildCount() - 1);
            }
        }

        // Initialize the UI with the existing array elements
        for (int i = 0; i < array.Length; i++)
        { 
            object value = array.GetValue(i);

            Control control = CreateControlForType(value, elementType, debugExportSpinBoxes, v =>
            {
                array.SetValue(v, i);
                valueChanged(array);
            });

            if (control != null)
            {
                SetControlValue(control, value);

                Button minusButton = new() { Text = "-" };

                HBoxContainer hbox = new();

                minusButton.Pressed += () =>
                {
                    int indexToRemove = minusButton.GetParent().GetIndex();
                    arrayVBox.RemoveChild(hbox);
                    array = array.RemoveAt(indexToRemove);
                    valueChanged(array);
                };

                hbox.AddChild(control);
                hbox.AddChild(minusButton);
                arrayVBox.AddChild(hbox);
            }
        }

        // Add a button to add more entries
        addButton.Pressed += AddNewEntryToArray;
        arrayVBox.AddChild(addButton);

        return arrayVBox;
    }

    private static Control CreateStringNameControl(object initialValue, Action<StringName> valueChanged)
    {
        StringName stringName = (StringName)initialValue;
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

    private static Control CreateNodePathControl(object initialValue, Action<NodePath> valueChanged)
    {
        NodePath nodePath = (NodePath)initialValue;

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

    private static Control CreateObjectControl(object initialValue, Action<object> valueChanged)
    {
        LineEdit lineEdit = new()
        {
            Text = initialValue?.ToString() ?? string.Empty
        };

        lineEdit.TextChanged += text =>
        {
            valueChanged(text);
        };

        return lineEdit;
    }

    private static Control CreateQuaternionControl(object initialValue, Action<Quaternion> valueChanged)
    {
        HBoxContainer quaternionHBox = new();

        Quaternion quaternion = (Quaternion)initialValue;

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

    private static Control CreateVector2Control(object initialValue, Action<Vector2> valueChanged)
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

        vector2HBox.AddChild(new GLabel("X"));
        vector2HBox.AddChild(spinBoxX);
        vector2HBox.AddChild(new GLabel("Y"));
        vector2HBox.AddChild(spinBoxY);

        return vector2HBox;
    }

    private static Control CreateVector2IControl(object initialValue, Action<Vector2I> valueChanged)
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

        vector2IHBox.AddChild(new GLabel("X"));
        vector2IHBox.AddChild(spinBoxX);
        vector2IHBox.AddChild(new GLabel("Y"));
        vector2IHBox.AddChild(spinBoxY);

        return vector2IHBox;
    }

    private static Control CreateVector3Control(object initialValue, Action<Vector3> valueChanged)
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

        vector3HBox.AddChild(new GLabel("X"));
        vector3HBox.AddChild(spinBoxX);
        vector3HBox.AddChild(new GLabel("Y"));
        vector3HBox.AddChild(spinBoxY);
        vector3HBox.AddChild(new GLabel("Z"));
        vector3HBox.AddChild(spinBoxZ);

        return vector3HBox;
    }

    private static Control CreateVector3IControl(object initialValue, Action<Vector3I> valueChanged)
    {
        HBoxContainer vector3IHBox = new();

        Vector3I vector3I = (Vector3I)initialValue;

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

    private static Control CreateVector4Control(object initialValue, Action<Vector4> valueChanged)
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

    private static Control CreateVector4IControl(object initialValue, Action<Vector4I> valueChanged)
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

    private static Control CreateNumericControl(object initialValue, Type type, List<DebugVisualSpinBox> debugExportSpinBoxes, Action<object> valueChanged)
    {
        SpinBox spinBox = CreateSpinBox(type);

        spinBox.Value = Convert.ToDouble(initialValue);
        spinBox.ValueChanged += value =>
        {
            object convertedValue = Convert.ChangeType(value, type);
            valueChanged(convertedValue);
        };

        return spinBox;
    }

    private static Control CreateBoolControl(object initialValue, Action<bool> valueChanged)
    {
        CheckBox checkBox = new()
        {
            ButtonPressed = (bool)initialValue
        };
        checkBox.Toggled += value => valueChanged(value);

        return checkBox;
    }

    private static Control CreateColorControl(object initialValue, Action<Color> valueChanged)
    {
        Color initialColor = (Color)initialValue;

        GColorPickerButton colorPickerButton = new(initialColor);
        colorPickerButton.OnColorChanged += color => valueChanged(color);

        return colorPickerButton.Control;
    }

    private static Control CreateStringControl(object initialValue, Action<string> valueChanged)
    {
        LineEdit lineEdit = new()
        {
            Text = initialValue.ToString()
        };

        lineEdit.TextChanged += text => valueChanged(text);

        return lineEdit;
    }

    private static Control CreateEnumControl(object initialValue, Type type, Action<object> valueChanged)
    {
        GOptionButtonEnum optionButton = new(type);
        optionButton.Select(initialValue);
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

    private static object CreateDefaultValue(Type type)
    {
        // Examples of Value Types: https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/builtin-types/value-types#kinds-of-value-types-and-type-constraints
        if (type.IsValueType)
        {
            return Activator.CreateInstance(type);
        }
        // Examples of Reference Types: https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/keywords/reference-types
        else if (type == typeof(string))
        {
            return string.Empty;
        }

        return null;
    }

    private static void SetControlValue(Control control, object value)
    {
        if (control is ColorPickerButton colorPickerButton)
        {
            colorPickerButton.Color = (Color)value;
        }
        else if (control is LineEdit lineEdit)
        {
            lineEdit.Text = (string)value;
        }

        // Implement more control types here
    }

    // Helper method to remove an element from an array
    private static Array RemoveAt(this Array source, int index)
    {
        if (source == null)
        {
            throw new ArgumentNullException(nameof(source));
        }
            
        if (index < 0 || index >= source.Length)
        {
            throw new ArgumentOutOfRangeException(nameof(index), index, $"Index was out of range");
        }   

        Array dest = Array.CreateInstance(source.GetType().GetElementType(), source.Length - 1);
        Array.Copy(source, 0, dest, 0, index);
        Array.Copy(source, index + 1, dest, index, source.Length - index - 1);

        return dest;
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

    private static Button CreateMethodButton(MethodInfo method, object target, ParameterInfo[] paramInfos, object[] providedValues)
    {
        Button button = new()
        {
            Text = method.Name,
            SizeFlagsHorizontal = SizeFlags.ShrinkCenter
        };

        button.Pressed += () =>
        {
            object[] parameters = ParameterConverter.ConvertParameterInfoToObjectArray(paramInfos, providedValues);

            method.Invoke(target, parameters);
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

        Type type = VisualNodeHandler.GetMemberType(member);

        object initialValue = VisualNodeHandler.GetMemberValue(member, node);

        Control element = CreateControlForType(initialValue, type, debugExportSpinBoxes, v =>
        {
            VisualNodeHandler.SetMemberValue(member, node, v);
        });

        if (element != null)
        {
            GLabel label = new()
            {
                Text = member.Name.ToPascalCase().AddSpaceBeforeEachCapital(),
                SizeFlagsHorizontal = SizeFlags.ExpandFill
            };

            hbox.AddChild(label);
            hbox.AddChild(element);
        }

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
