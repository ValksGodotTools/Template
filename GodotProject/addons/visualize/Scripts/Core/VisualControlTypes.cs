using Godot;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Visualize.Utils;
using static Godot.Control;

namespace Visualize.Core;

public class VisualControlInfo(IVisualControl control, List<Control> controls)
{
    public IVisualControl Control { get; } = control;
    public List<Control> Controls { get; } = controls;
}

public static class VisualControlTypes
{
    public static VisualControlInfo CreateControlForType(object initialValue, Type type, List<VisualSpinBox> debugExportSpinBoxes, Action<object> valueChanged)
    {
        VisualControlInfo info = type switch
        {
            _ when type == typeof(bool) => Bool(initialValue, v => valueChanged(v)),
            _ when type == typeof(string) => String(initialValue, v => valueChanged(v)),
            _ when type == typeof(object) => Object(initialValue, v => valueChanged(v)),
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
            _ when type.IsNumericType() => Numeric(initialValue, type, debugExportSpinBoxes, v => valueChanged(v)),
            _ when type.IsEnum => Enum(initialValue, type, v => valueChanged(v)),
            _ when type.IsArray => Array(initialValue, type, debugExportSpinBoxes, v => valueChanged(v)),
            _ when type.IsGenericType && type.GetGenericTypeDefinition() == typeof(List<>) => List(initialValue, type, debugExportSpinBoxes, v => valueChanged(v)),
            _ when type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Dictionary<,>) => Dictionary(initialValue, type, debugExportSpinBoxes, v => valueChanged(v)),
            _ when type.IsClass && !type.IsSubclassOf(typeof(GodotObject)) => Class(initialValue, type, debugExportSpinBoxes, v => valueChanged(v)),
            _ when type.IsValueType && !type.IsClass && !type.IsSubclassOf(typeof(GodotObject)) => Class(initialValue, type, debugExportSpinBoxes, v => valueChanged(v)),
            _ => new VisualControlInfo(null, null)
        };

        if (info.Control == null)
        {
            GD.PushWarning($"The type '{type}' is not supported for the {nameof(VisualizeAttribute)}");
        }

        return info;
    }

    private static VisualControlInfo Class(object target, Type type, List<VisualSpinBox> debugExportSpinBoxes, Action<object> valueChanged)
    {
        List<Control> controls = [];
        BindingFlags flags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly;

        PropertyInfo[] properties = type.GetProperties(flags);
        FieldInfo[] allFields = type.GetFields(flags);
        FieldInfo[] nonBackingFields = allFields.Where(f => !f.Name.StartsWith("<") || !f.Name.EndsWith(">k__BackingField")).ToArray();
        MethodInfo[] allMethods = type.GetMethods(flags);
        MethodInfo[] nonPropertyMethods = allMethods.Where(m => !m.Name.StartsWith("get_") && !m.Name.StartsWith("set_")).ToArray();

        VBoxContainer vbox = new();

        foreach (PropertyInfo property in properties)
        {
            object initialValue = property.GetValue(target);
            VisualControlInfo control = CreateControlForType(initialValue, property.PropertyType, debugExportSpinBoxes, v =>
            {
                property.SetValue(target, v);
                valueChanged(target);
            });

            control.Controls.ForEach(controls.Add);

            if (control.Control != null)
            {
                HBoxContainer hbox = new();
                hbox.AddChild(new Label { Text = property.Name.ToPascalCase().AddSpaceBeforeEachCapital() });
                hbox.AddChild(control.Control.Control);
                vbox.AddChild(hbox);
            }
        }

        foreach (FieldInfo field in nonBackingFields)
        {
            object initialValue = field.GetValue(target);
            VisualControlInfo control = CreateControlForType(initialValue, field.FieldType, debugExportSpinBoxes, v =>
            {
                field.SetValue(target, v);
                valueChanged(target);
            });

            control.Controls.ForEach(controls.Add);

            if (control.Control != null)
            {
                if (control.Control.Control is SpinBox spinBox)
                {
                    spinBox.Editable = !field.IsLiteral;
                }
                else if (control.Control.Control is LineEdit lineEdit)
                {
                    lineEdit.Editable = !field.IsLiteral;
                }
                else if (control.Control.Control is BaseButton baseButton)
                {
                    baseButton.Disabled = field.IsLiteral;
                }

                HBoxContainer hbox = new();
                hbox.AddChild(new Label { Text = field.Name.ToPascalCase().AddSpaceBeforeEachCapital() });
                hbox.AddChild(control.Control.Control);
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

        return new VisualControlInfo(new VBoxContainerControl(vbox), controls);
    }

    private static VisualControlInfo Dictionary(object initialValue, Type type, List<VisualSpinBox> debugExportSpinBoxes, Action<IDictionary> valueChanged)
    {
        List<Control> controls = [];
        VBoxContainer dictionaryVBox = new() { SizeFlagsHorizontal = SizeFlags.ShrinkEnd | SizeFlags.Expand };
        Button addButton = new() { Text = "+" };

        Type[] genericArguments = type.GetGenericArguments();
        Type keyType = genericArguments[0];
        Type valueType = genericArguments[1];

        object defaultKey = VisualMethods.CreateDefaultValue(keyType);
        object defaultValue = VisualMethods.CreateDefaultValue(valueType);

        IDictionary dictionary = initialValue as IDictionary ?? (IDictionary)Activator.CreateInstance(typeof(Dictionary<,>).MakeGenericType(keyType, valueType));

        foreach (DictionaryEntry entry in dictionary)
        {
            object key = entry.Key;
            object value = entry.Value;

            VisualControlInfo valueControl = CreateControlForType(value, valueType, debugExportSpinBoxes, v =>
            {
                dictionary[key] = v;
                valueChanged(dictionary);
            });

            valueControl.Controls.ForEach(controls.Add);

            VisualControlInfo keyControl = CreateControlForType(key, keyType, debugExportSpinBoxes, v =>
            {
                if (dictionary.Contains(v))
                    return;
                if (v.GetType() != keyType)
                    throw new ArgumentException($"Type mismatch: Expected {keyType}, got {v.GetType()}");

                dictionary.Remove(key);
                dictionary[v] = value;
                key = v;
                valueChanged(dictionary);
                SetControlValue(valueControl.Control.Control, defaultValue);
            });

            keyControl.Controls.ForEach(controls.Add);

            if (keyControl.Control != null && valueControl.Control != null)
            {
                SetControlValue(keyControl.Control.Control, key);
                SetControlValue(valueControl.Control.Control, value);

                Button removeKeyEntryButton = new() { Text = "-" };
                HBoxContainer hbox = new();

                removeKeyEntryButton.Pressed += () =>
                {
                    dictionaryVBox.RemoveChild(hbox);
                    dictionary.Remove(key);
                    valueChanged(dictionary);
                };

                hbox.AddChild(keyControl.Control.Control);
                hbox.AddChild(valueControl.Control.Control);
                hbox.AddChild(removeKeyEntryButton);
                dictionaryVBox.AddChild(hbox);
            }
        }

        void AddNewEntryToDictionary()
        {
            if (dictionary.Contains(defaultKey))
                return;
            dictionary[defaultKey] = defaultValue;
            valueChanged(dictionary);

            object oldKey = defaultKey;

            VisualControlInfo valueControl = CreateControlForType(defaultValue, valueType, debugExportSpinBoxes, v =>
            {
                dictionary[oldKey] = v;
                valueChanged(dictionary);
            });

            valueControl.Controls.ForEach(controls.Add);

            VisualControlInfo keyControl = CreateControlForType(defaultKey, keyType, debugExportSpinBoxes, v =>
            {
                if (dictionary.Contains(v))
                    return;
                if (v.GetType() != keyType)
                    throw new ArgumentException($"Type mismatch: Expected {keyType}, got {v.GetType()}");

                dictionary.Remove(oldKey);
                dictionary[v] = defaultValue;
                oldKey = v;
                valueChanged(dictionary);
                SetControlValue(valueControl.Control.Control, defaultValue);
            });

            keyControl.Controls.ForEach(controls.Add);

            if (keyControl.Control != null && valueControl.Control != null)
            {
                Button removeKeyEntryButton = new() { Text = "-" };
                HBoxContainer hbox = new();

                removeKeyEntryButton.Pressed += () =>
                {
                    dictionaryVBox.RemoveChild(hbox);
                    dictionary.Remove(oldKey);
                    valueChanged(dictionary);
                };

                hbox.AddChild(keyControl.Control.Control);
                hbox.AddChild(valueControl.Control.Control);
                hbox.AddChild(removeKeyEntryButton);
                dictionaryVBox.AddChild(hbox);
                dictionaryVBox.MoveChild(addButton, dictionaryVBox.GetChildCount() - 1);
            }
        }

        addButton.Pressed += AddNewEntryToDictionary;
        dictionaryVBox.AddChild(addButton);

        return new VisualControlInfo(new VBoxContainerControl(dictionaryVBox), controls);
    }

    private static VisualControlInfo List(object initialValue, Type type, List<VisualSpinBox> debugExportSpinBoxes, Action<IList> valueChanged)
    {
        List<Control> controls = [];
        VBoxContainer listVBox = new() { SizeFlagsHorizontal = SizeFlags.ShrinkEnd | SizeFlags.Expand };
        Button addButton = new() { Text = "+" };

        Type elementType = type.GetGenericArguments()[0];
        IList list = initialValue as IList ?? (IList)Activator.CreateInstance(typeof(List<>).MakeGenericType(elementType));

        void AddNewEntryToList()
        {
            object newValue = VisualMethods.CreateDefaultValue(elementType);
            list.Add(newValue);
            valueChanged(list);

            int newIndex = list.Count - 1;

            VisualControlInfo control = CreateControlForType(newValue, elementType, debugExportSpinBoxes, v =>
            {
                list[newIndex] = v;
                valueChanged(list);
            });

            control.Controls.ForEach(controls.Add);

            if (control.Control != null)
            {
                HBoxContainer hbox = new();
                Button minusButton = new() { Text = "-" };

                minusButton.Pressed += () =>
                {
                    int indexToRemove = minusButton.GetParent().GetIndex();
                    listVBox.RemoveChild(hbox);
                    list.RemoveAt(indexToRemove);
                    valueChanged(list);
                };

                hbox.AddChild(control.Control.Control);
                hbox.AddChild(minusButton);
                listVBox.AddChild(hbox);
                listVBox.MoveChild(addButton, listVBox.GetChildCount() - 1);
            }
        }

        for (int i = 0; i < list.Count; i++)
        {
            object value = list[i];
            VisualControlInfo control = CreateControlForType(value, elementType, debugExportSpinBoxes, v =>
            {
                list[i] = v;
                valueChanged(list);
            });

            if (control.Control != null)
            {
                SetControlValue(control.Control.Control, value);

                Button minusButton = new() { Text = "-" };
                HBoxContainer hbox = new();

                minusButton.Pressed += () =>
                {
                    int indexToRemove = minusButton.GetParent().GetIndex();
                    listVBox.RemoveChild(hbox);
                    list.RemoveAt(indexToRemove);
                    valueChanged(list);
                };

                hbox.AddChild(control.Control.Control);
                hbox.AddChild(minusButton);
                listVBox.AddChild(hbox);
            }
        }

        addButton.Pressed += AddNewEntryToList;
        listVBox.AddChild(addButton);

        return new VisualControlInfo(new VBoxContainerControl(listVBox), controls);
    }

    private static VisualControlInfo Array(object initialValue, Type type, List<VisualSpinBox> debugExportSpinBoxes, Action<Array> valueChanged)
    {
        List<Control> controls = [];
        VBoxContainer arrayVBox = new() { SizeFlagsHorizontal = SizeFlags.ShrinkEnd | SizeFlags.Expand };
        Button addButton = new() { Text = "+" };

        Type elementType = type.GetElementType();
        Array array = initialValue as Array ?? System.Array.CreateInstance(elementType, 0);

        void AddNewEntryToArray()
        {
            Array newArray = System.Array.CreateInstance(elementType, array.Length + 1);
            System.Array.Copy(array, newArray, array.Length);
            array = newArray;
            valueChanged(array);

            object newValue = VisualMethods.CreateDefaultValue(elementType);
            int newIndex = array.Length - 1;

            VisualControlInfo control = CreateControlForType(newValue, elementType, debugExportSpinBoxes, v =>
            {
                array.SetValue(v, newIndex);
                valueChanged(array);
            });

            control.Controls.ForEach(controls.Add);

            if (control.Control != null)
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

                hbox.AddChild(control.Control.Control);
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

            control.Controls.ForEach(controls.Add);

            if (control.Control != null)
            {
                SetControlValue(control.Control.Control, value);

                Button minusButton = new() { Text = "-" };
                HBoxContainer hbox = new();

                minusButton.Pressed += () =>
                {
                    int indexToRemove = minusButton.GetParent().GetIndex();
                    arrayVBox.RemoveChild(hbox);
                    array = array.RemoveAt(indexToRemove);
                    valueChanged(array);
                };

                hbox.AddChild(control.Control.Control);
                hbox.AddChild(minusButton);
                arrayVBox.AddChild(hbox);
            }
        }

        addButton.Pressed += AddNewEntryToArray;
        arrayVBox.AddChild(addButton);

        return new VisualControlInfo(new VBoxContainerControl(arrayVBox), controls);
    }

    private static VisualControlInfo StringName(object initialValue, Action<StringName> valueChanged)
    {
        List<Control> controls = [];
        StringName stringName = (StringName)initialValue;
        string initialText = stringName != null ? stringName.ToString() : string.Empty;

        LineEdit lineEdit = new() { Text = initialText };
        lineEdit.TextChanged += text => valueChanged(new StringName(text));

        controls.Add(lineEdit);

        return new VisualControlInfo(new LineEditControl(lineEdit), controls);
    }

    private static VisualControlInfo NodePath(object initialValue, Action<NodePath> valueChanged)
    {
        List<Control> controls = [];
        NodePath nodePath = (NodePath)initialValue;
        string initialText = nodePath != null ? nodePath.ToString() : string.Empty;

        LineEdit lineEdit = new() { Text = initialText };
        lineEdit.TextChanged += text => valueChanged(new NodePath(text));

        controls.Add(lineEdit);

        return new VisualControlInfo(new LineEditControl(lineEdit), controls);
    }

    private static VisualControlInfo Object(object initialValue, Action<object> valueChanged)
    {
        List<Control> controls = [];
        LineEdit lineEdit = new() { Text = initialValue?.ToString() ?? string.Empty };
        lineEdit.TextChanged += text => valueChanged(text);

        controls.Add(lineEdit);

        return new VisualControlInfo(new LineEditControl(lineEdit), controls);
    }

    private static VisualControlInfo Quaternion(object initialValue, Action<Quaternion> valueChanged)
    {
        List<Control> controls = [];
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

        controls.Add(spinBoxX);
        controls.Add(spinBoxY);
        controls.Add(spinBoxZ);
        controls.Add(spinBoxW);

        return new VisualControlInfo(new QuaternionControl(quaternionHBox, spinBoxX, spinBoxY, spinBoxZ, spinBoxW), controls);
    }

    private static VisualControlInfo Vector2(object initialValue, Action<Vector2> valueChanged)
    {
        List<Control> controls = [];
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

        controls.Add(spinBoxX);
        controls.Add(spinBoxY);

        return new VisualControlInfo(new Vector2Control(vector2HBox, spinBoxX, spinBoxY), controls);
    }

    private static VisualControlInfo Vector2I(object initialValue, Action<Vector2I> valueChanged)
    {
        List<Control> controls = [];
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

        controls.Add(spinBoxX);
        controls.Add(spinBoxY);

        return new VisualControlInfo(new Vector2IControl(vector2IHBox, spinBoxX, spinBoxY), controls);
    }

    private static VisualControlInfo Vector3(object initialValue, Action<Vector3> valueChanged)
    {
        List<Control> controls = [];
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

        controls.Add(spinBoxX);
        controls.Add(spinBoxY);
        controls.Add(spinBoxZ);

        return new VisualControlInfo(new Vector3Control(vector3HBox, spinBoxX, spinBoxY, spinBoxZ), controls);
    }

    private static VisualControlInfo Vector3I(object initialValue, Action<Vector3I> valueChanged)
    {
        List<Control> controls = [];
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

        controls.Add(spinBoxX);
        controls.Add(spinBoxY);
        controls.Add(spinBoxZ);

        return new VisualControlInfo(new Vector3IControl(vector3IHBox, spinBoxX, spinBoxY, spinBoxZ), controls);
    }

    private static VisualControlInfo Vector4(object initialValue, Action<Vector4> valueChanged)
    {
        List<Control> controls = [];
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

        controls.Add(spinBoxX);
        controls.Add(spinBoxY);
        controls.Add(spinBoxZ);
        controls.Add(spinBoxW);

        return new VisualControlInfo(new Vector4Control(vector4HBox, spinBoxX, spinBoxY, spinBoxZ, spinBoxW), controls);
    }

    private static VisualControlInfo Vector4I(object initialValue, Action<Vector4I> valueChanged)
    {
        List<Control> controls = [];
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

        controls.Add(spinBoxX);
        controls.Add(spinBoxY);
        controls.Add(spinBoxZ);
        controls.Add(spinBoxW);

        return new VisualControlInfo(new Vector4IControl(vector4IHBox, spinBoxX, spinBoxY, spinBoxZ, spinBoxW), controls);
    }

    private static VisualControlInfo Numeric(object initialValue, Type type, List<VisualSpinBox> debugExportSpinBoxes, Action<object> valueChanged)
    {
        List<Control> controls = [];
        SpinBox spinBox = CreateSpinBox(type);

        spinBox.Value = Convert.ToDouble(initialValue);
        spinBox.ValueChanged += value =>
        {
            object convertedValue = Convert.ChangeType(value, type);
            valueChanged(convertedValue);
        };

        controls.Add(spinBox);

        return new VisualControlInfo(new SpinBoxControl(spinBox), controls);
    }

    private static VisualControlInfo Bool(object initialValue, Action<bool> valueChanged)
    {
        List<Control> controls = [];
        CheckBox checkBox = new() { ButtonPressed = (bool)initialValue };
        checkBox.Toggled += value => valueChanged(value);

        controls.Add(checkBox);

        return new VisualControlInfo(new CheckBoxControl(checkBox), controls);
    }

    private static VisualControlInfo Color(object initialValue, Action<Color> valueChanged)
    {
        List<Control> controls = [];
        Color initialColor = (Color)initialValue;

        GColorPickerButton colorPickerButton = new(initialColor);
        colorPickerButton.OnColorChanged += color => valueChanged(color);

        controls.Add(colorPickerButton.Control);

        return new VisualControlInfo(new ColorPickerButtonControl(colorPickerButton), controls);
    }

    private static VisualControlInfo String(object initialValue, Action<string> valueChanged)
    {
        List<Control> controls = [];
        LineEdit lineEdit = new() { Text = initialValue.ToString() };
        lineEdit.TextChanged += text => valueChanged(text);

        controls.Add(lineEdit);

        return new VisualControlInfo(new LineEditControl(lineEdit), controls);
    }

    private static VisualControlInfo Enum(object initialValue, Type type, Action<object> valueChanged)
    {
        List<Control> controls = [];
        GOptionButtonEnum optionButton = new(type);
        optionButton.Select(initialValue);
        optionButton.OnItemSelected += item => valueChanged(item);

        controls.Add(optionButton.Control);

        return new VisualControlInfo(new OptionButtonEnumControl(optionButton), controls);
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
        else if (control is SpinBox spinBox)
        {
            spinBox.Value = Convert.ToDouble(value);
        }
        else if (control is CheckBox checkBox)
        {
            checkBox.ButtonPressed = (bool)value;
        }
        else if (control is OptionButton optionButton)
        {
            optionButton.Select((int)value);
        }
        else if (control is ColorPickerButton colorPicker)
        {
            colorPicker.Color = (Color)value;
        }
        // Add more control types here as needed
    }

    // Helper method to remove an element from an array
    private static Array RemoveAt(this Array source, int index)
    {
        ArgumentNullException.ThrowIfNull(source);

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

public interface IVisualControl
{
    void SetValue(object value);
    Control Control { get; }
    void SetEditable(bool editable);
}

public class Vector2Control(HBoxContainer vector2HBox, SpinBox spinBoxX, SpinBox spinBoxY) : IVisualControl
{
    private readonly HBoxContainer _vector2HBox = vector2HBox;
    private readonly SpinBox _spinBoxX = spinBoxX;
    private readonly SpinBox _spinBoxY = spinBoxY;

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

public class Vector2IControl(HBoxContainer vector2IHBox, SpinBox spinBoxX, SpinBox spinBoxY) : IVisualControl
{
    private readonly HBoxContainer _vector2IHBox = vector2IHBox;
    private readonly SpinBox _spinBoxX = spinBoxX;
    private readonly SpinBox _spinBoxY = spinBoxY;

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

public class Vector3Control(HBoxContainer vector3HBox, SpinBox spinBoxX, SpinBox spinBoxY, SpinBox spinBoxZ) : IVisualControl
{
    private readonly HBoxContainer _vector3HBox = vector3HBox;
    private readonly SpinBox _spinBoxX = spinBoxX;
    private readonly SpinBox _spinBoxY = spinBoxY;
    private readonly SpinBox _spinBoxZ = spinBoxZ;

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

public class Vector3IControl(HBoxContainer vector3IHBox, SpinBox spinBoxX, SpinBox spinBoxY, SpinBox spinBoxZ) : IVisualControl
{
    private readonly HBoxContainer _vector3IHBox = vector3IHBox;
    private readonly SpinBox _spinBoxX = spinBoxX;
    private readonly SpinBox _spinBoxY = spinBoxY;
    private readonly SpinBox _spinBoxZ = spinBoxZ;

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

public class Vector4Control(HBoxContainer vector4HBox, SpinBox spinBoxX, SpinBox spinBoxY, SpinBox spinBoxZ, SpinBox spinBoxW) : IVisualControl
{
    private readonly HBoxContainer _vector4HBox = vector4HBox;
    private readonly SpinBox _spinBoxX = spinBoxX;
    private readonly SpinBox _spinBoxY = spinBoxY;
    private readonly SpinBox _spinBoxZ = spinBoxZ;
    private readonly SpinBox _spinBoxW = spinBoxW;

    public void SetValue(object value)
    {
        if (value is Vector4 vector4)
        {
            _spinBoxX.Value = vector4.X;
            _spinBoxY.Value = vector4.Y;
            _spinBoxZ.Value = vector4.Z;
            _spinBoxW.Value = vector4.W;
        }
    }

    public Control Control => _vector4HBox;

    public void SetEditable(bool editable)
    {
        _spinBoxX.Editable = editable;
        _spinBoxY.Editable = editable;
        _spinBoxZ.Editable = editable;
        _spinBoxW.Editable = editable;
    }
}

public class Vector4IControl(HBoxContainer vector4IHBox, SpinBox spinBoxX, SpinBox spinBoxY, SpinBox spinBoxZ, SpinBox spinBoxW) : IVisualControl
{
    private readonly HBoxContainer _vector4IHBox = vector4IHBox;
    private readonly SpinBox _spinBoxX = spinBoxX;
    private readonly SpinBox _spinBoxY = spinBoxY;
    private readonly SpinBox _spinBoxZ = spinBoxZ;
    private readonly SpinBox _spinBoxW = spinBoxW;

    public void SetValue(object value)
    {
        if (value is Vector4I vector4I)
        {
            _spinBoxX.Value = vector4I.X;
            _spinBoxY.Value = vector4I.Y;
            _spinBoxZ.Value = vector4I.Z;
            _spinBoxW.Value = vector4I.W;
        }
    }

    public Control Control => _vector4IHBox;

    public void SetEditable(bool editable)
    {
        _spinBoxX.Editable = editable;
        _spinBoxY.Editable = editable;
        _spinBoxZ.Editable = editable;
        _spinBoxW.Editable = editable;
    }
}

public class QuaternionControl(HBoxContainer quaternionHBox, SpinBox spinBoxX, SpinBox spinBoxY, SpinBox spinBoxZ, SpinBox spinBoxW) : IVisualControl
{
    private readonly HBoxContainer _quaternionHBox = quaternionHBox;
    private readonly SpinBox _spinBoxX = spinBoxX;
    private readonly SpinBox _spinBoxY = spinBoxY;
    private readonly SpinBox _spinBoxZ = spinBoxZ;
    private readonly SpinBox _spinBoxW = spinBoxW;

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

public class LineEditControl(LineEdit lineEdit) : IVisualControl
{
    private readonly LineEdit _lineEdit = lineEdit;

    public void SetValue(object value)
    {
        if (value is string text)
        {
            _lineEdit.Text = text;
        }
    }

    public Control Control => _lineEdit;

    public void SetEditable(bool editable)
    {
        _lineEdit.Editable = editable;
    }
}

public class SpinBoxControl(SpinBox spinBox) : IVisualControl
{
    private readonly SpinBox _spinBox = spinBox;

    public void SetValue(object value)
    {
        if (value != null)
        {
            try
            {
                _spinBox.Value = Convert.ToDouble(value);
            }
            catch (InvalidCastException)
            {
                // Handle the case where the value cannot be converted to double
                GD.PushWarning($"Cannot convert value of type {value.GetType()} to double.");
            }
        }
    }

    public Control Control => _spinBox;

    public void SetEditable(bool editable)
    {
        _spinBox.Editable = editable;
    }
}

public class CheckBoxControl(CheckBox checkBox) : IVisualControl
{
    private readonly CheckBox _checkBox = checkBox;

    public void SetValue(object value)
    {
        if (value is bool boolValue)
        {
            _checkBox.ButtonPressed = boolValue;
        }
    }

    public Control Control => _checkBox;

    public void SetEditable(bool editable)
    {
        _checkBox.Disabled = !editable;
    }
}

public class ColorPickerButtonControl(GColorPickerButton colorPickerButton) : IVisualControl
{
    private readonly GColorPickerButton _colorPickerButton = colorPickerButton;

    public void SetValue(object value)
    {
        if (value is Color color)
        {
            _colorPickerButton.Control.Color = color;
        }
    }

    public Control Control => _colorPickerButton.Control;

    public void SetEditable(bool editable)
    {
        _colorPickerButton.Control.Disabled = !editable;
    }
}

public class OptionButtonEnumControl(GOptionButtonEnum optionButton) : IVisualControl
{
    private readonly GOptionButtonEnum _optionButton = optionButton;

    public void SetValue(object value)
    {
        _optionButton.Select(value);
    }

    public Control Control => _optionButton.Control;

    public void SetEditable(bool editable)
    {
        _optionButton.Control.Disabled = !editable;
    }
}

public class VBoxContainerControl(VBoxContainer vboxContainer) : IVisualControl
{
    private readonly VBoxContainer _vboxContainer = vboxContainer;

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
