using Godot;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Visualize.Utils;
using static Godot.Control;

namespace Visualize;

public static class VisualControlTypes
{
    public static Control CreateControlForType(object initialValue, Type type, List<VisualSpinBox> debugExportSpinBoxes, Action<object> valueChanged)
    {
        // Order of which these are handled matters
        Control control = type switch
        {
            // Handle C# specific types
            _ when type == typeof(bool) => Bool(initialValue, v => valueChanged(v)),
            _ when type == typeof(string) => String(initialValue, v => valueChanged(v)),
            _ when type == typeof(object) => Object(initialValue, v => valueChanged(v)),

            // Handle Godot specific types
            _ when type == typeof(Color) => Color(initialValue, v => valueChanged(v)),
            _ when type == typeof(Vector2) => Vector2(initialValue, v => valueChanged(v)),
            _ when type == typeof(Vector2I) => Vector2I(initialValue, v => valueChanged(v)),
            _ when type == typeof(Vector3) => Vector3(initialValue, v => valueChanged(v)),
            _ when type == typeof(Vector3I) => Vector3I(initialValue, v => valueChanged(v)),
            _ when type == typeof(Vector4) => Vector4(initialValue, v => valueChanged(v)),
            _ when type == typeof(Vector4I) => Vector4I(initialValue, v => valueChanged(v)),
            _ when type == typeof(Quaternion) => Quaternion(initialValue, v => valueChanged(v)),
            _ when type == typeof(NodePath) => NodePath(initialValue, v => valueChanged(v)),
            _ when type == typeof(StringName) => StringName(initialValue, v => valueChanged(v)),

            // Handle numeric, enum and array types
            _ when type.IsNumericType() => Numeric(initialValue, type, debugExportSpinBoxes, v => valueChanged(v)),
            _ when type.IsEnum => Enum(initialValue, type, v => valueChanged(v)),
            _ when type.IsArray => Array(initialValue, type, debugExportSpinBoxes, v => valueChanged(v)),
            _ when type.IsGenericType && type.GetGenericTypeDefinition() == typeof(List<>) => List(initialValue, type, debugExportSpinBoxes, v => valueChanged(v)),
            _ when type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Dictionary<,>) => Dictionary(initialValue, type, debugExportSpinBoxes, v => valueChanged(v)),
            _ when type.IsClass && !type.IsSubclassOf(typeof(GodotObject)) => Class(initialValue, type, debugExportSpinBoxes, v => valueChanged(v)),
            _ when type.IsValueType && !type.IsClass && !type.IsSubclassOf(typeof(GodotObject)) => Class(initialValue, type, debugExportSpinBoxes, v => valueChanged(v)),

            _ => null
        };

        if (control == null)
        {
            GD.PushWarning($"The type '{type}' is not supported for the {nameof(VisualizeAttribute)}");
        }

        return control;
    }

    private static Control Class(object target, Type type, List<VisualSpinBox> debugExportSpinBoxes, Action<object> valueChanged)
    {
        BindingFlags flags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly;

        PropertyInfo[] properties = type.GetProperties(flags);

        FieldInfo[] allFields = type.GetFields(flags);

        // Filter out fields that are backing fields for properties
        FieldInfo[] nonBackingFields = allFields
            .Where(f => !f.Name.StartsWith("<") || !f.Name.EndsWith(">k__BackingField"))
            .ToArray();

        MethodInfo[] allMethods = type.GetMethods(flags);

        // Filter out methods that are property accessors
        MethodInfo[] nonPropertyMethods = allMethods
            .Where(m => !m.Name.StartsWith("get_") && !m.Name.StartsWith("set_"))
            .ToArray();

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
                HBoxContainer hbox = new();
                hbox.AddChild(new Label { Text = property.Name.ToPascalCase().AddSpaceBeforeEachCapital() });
                hbox.AddChild(control);
                vbox.AddChild(hbox);
            }
        }

        foreach (FieldInfo field in nonBackingFields)
        {
            object initialValue = field.GetValue(target);

            Control control = CreateControlForType(initialValue, field.FieldType, debugExportSpinBoxes, v =>
            {
                field.SetValue(target, v);
                valueChanged(target);
            });

            if (control != null)
            {
                if (control is SpinBox spinBox)
                {
                    spinBox.Editable = !field.IsLiteral;
                }
                else if (control is LineEdit lineEdit)
                {
                    lineEdit.Editable = !field.IsLiteral;
                }
                else if (control is BaseButton baseButton)
                {
                    baseButton.Disabled = field.IsLiteral;
                }

                HBoxContainer hbox = new();
                hbox.AddChild(new Label { Text = field.Name.ToPascalCase().AddSpaceBeforeEachCapital() });
                hbox.AddChild(control);
                vbox.AddChild(hbox);
            }
        }

        foreach (MethodInfo method in nonPropertyMethods)
        {
            ParameterInfo[] paramInfos = method.GetParameters();
            object[] providedValues = new object[paramInfos.Length];

            HBoxContainer hboxParams = VisualMethods.CreateMethodParameterControls(method, debugExportSpinBoxes, providedValues);

            vbox.AddChild(hboxParams);

            Button button = VisualMethods.CreateMethodButton(method, target, paramInfos, providedValues);

            vbox.AddChild(button);
        }

        return vbox;
    }

    private static Control Dictionary(object initialValue, Type type, List<VisualSpinBox> debugExportSpinBoxes, Action<IDictionary> valueChanged)
    {
        VBoxContainer dictionaryVBox = new()
        {
            SizeFlagsHorizontal = SizeFlags.ShrinkEnd | SizeFlags.Expand
        };

        Button addButton = new() { Text = "+" };

        Type[] genericArguments = type.GetGenericArguments();

        Type keyType = genericArguments[0];
        Type valueType = genericArguments[1];

        object defaultKey = VisualMethods.CreateDefaultValue(keyType);
        object defaultValue = VisualMethods.CreateDefaultValue(valueType);

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

                Button removeKeyEntryButton = new()
                {
                    Text = "-"
                };

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
                Button removeKeyEntryButton = new();
                removeKeyEntryButton.Text = "-";

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

    private static Control List(object initialValue, Type type, List<VisualSpinBox> debugExportSpinBoxes, Action<IList> valueChanged)
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
            object newValue = VisualMethods.CreateDefaultValue(elementType);
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

    private static Control Array(object initialValue, Type type, List<VisualSpinBox> debugExportSpinBoxes, Action<Array> valueChanged)
    {
        VBoxContainer arrayVBox = new()
        {
            SizeFlagsHorizontal = SizeFlags.ShrinkEnd | SizeFlags.Expand
        };

        Button addButton = new() { Text = "+" };

        Type elementType = type.GetElementType();
        Array array = initialValue as Array ?? System.Array.CreateInstance(elementType, 0);

        void AddNewEntryToArray()
        {
            // Create a new array with the updated length
            Array newArray = System.Array.CreateInstance(elementType, array.Length + 1);
            System.Array.Copy(array, newArray, array.Length);
            array = newArray;
            valueChanged(array);

            object newValue = VisualMethods.CreateDefaultValue(elementType);

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

    private static Control StringName(object initialValue, Action<StringName> valueChanged)
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

    private static Control NodePath(object initialValue, Action<NodePath> valueChanged)
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

    private static Control Object(object initialValue, Action<object> valueChanged)
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

    private static Control Quaternion(object initialValue, Action<Quaternion> valueChanged)
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

        quaternionHBox.AddChild(new Label { Text = "X" });
        quaternionHBox.AddChild(spinBoxX);
        quaternionHBox.AddChild(new Label { Text = "Y" });
        quaternionHBox.AddChild(spinBoxY);
        quaternionHBox.AddChild(new Label { Text = "Z" });
        quaternionHBox.AddChild(spinBoxZ);
        quaternionHBox.AddChild(new Label { Text = "W" });
        quaternionHBox.AddChild(spinBoxW);

        return quaternionHBox;
    }

    private static Control Vector2(object initialValue, Action<Vector2> valueChanged)
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

        return vector2HBox;
    }

    private static Control Vector2I(object initialValue, Action<Vector2I> valueChanged)
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

        return vector2IHBox;
    }

    private static Control Vector3(object initialValue, Action<Vector3> valueChanged)
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

        return vector3HBox;
    }

    private static Control Vector3I(object initialValue, Action<Vector3I> valueChanged)
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

        vector3IHBox.AddChild(new Label { Text = "X" });
        vector3IHBox.AddChild(spinBoxX);
        vector3IHBox.AddChild(new Label { Text = "Y" });
        vector3IHBox.AddChild(spinBoxY);
        vector3IHBox.AddChild(new Label { Text = "Z" });
        vector3IHBox.AddChild(spinBoxZ);

        return vector3IHBox;
    }

    private static Control Vector4(object initialValue, Action<Vector4> valueChanged)
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

        vector4HBox.AddChild(new Label { Text = "X" });
        vector4HBox.AddChild(spinBoxX);
        vector4HBox.AddChild(new Label { Text = "Y" });
        vector4HBox.AddChild(spinBoxY);
        vector4HBox.AddChild(new Label { Text = "Z" });
        vector4HBox.AddChild(spinBoxZ);
        vector4HBox.AddChild(new Label { Text = "W" });
        vector4HBox.AddChild(spinBoxW);

        return vector4HBox;
    }

    private static Control Vector4I(object initialValue, Action<Vector4I> valueChanged)
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

        return vector4IHBox;
    }

    private static Control Numeric(object initialValue, Type type, List<VisualSpinBox> debugExportSpinBoxes, Action<object> valueChanged)
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

    private static Control Bool(object initialValue, Action<bool> valueChanged)
    {
        CheckBox checkBox = new()
        {
            ButtonPressed = (bool)initialValue
        };
        checkBox.Toggled += value => valueChanged(value);

        return checkBox;
    }

    private static Control Color(object initialValue, Action<Color> valueChanged)
    {
        Color initialColor = (Color)initialValue;

        GColorPickerButton colorPickerButton = new(initialColor);
        colorPickerButton.OnColorChanged += color => valueChanged(color);

        return colorPickerButton.Control;
    }

    private static Control String(object initialValue, Action<string> valueChanged)
    {
        LineEdit lineEdit = new()
        {
            Text = initialValue.ToString()
        };

        lineEdit.TextChanged += text => valueChanged(text);

        return lineEdit;
    }

    private static Control Enum(object initialValue, Type type, Action<object> valueChanged)
    {
        GOptionButtonEnum optionButton = new(type);
        optionButton.Select(initialValue);
        optionButton.OnItemSelected += item => valueChanged(item);

        return optionButton.Control;
    }

    #region Util Functions
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

        Array dest = System.Array.CreateInstance(source.GetType().GetElementType(), source.Length - 1);
        System.Array.Copy(source, 0, dest, 0, index);
        System.Array.Copy(source, index + 1, dest, index, source.Length - index - 1);

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
