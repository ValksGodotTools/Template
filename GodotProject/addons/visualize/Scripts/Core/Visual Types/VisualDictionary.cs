using Godot;
using System;
using System.Collections;
using System.Collections.Generic;
using static Godot.Control;

namespace Visualize.Core;

public static partial class VisualControlTypes
{
    private static VisualControlInfo VisualDictionary(object initialValue, Type type, List<VisualSpinBox> debugExportSpinBoxes, Action<IDictionary> valueChanged)
    {
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
                SetControlValue(valueControl.VisualControl.Control, defaultValue);
            });

            if (keyControl.VisualControl != null && valueControl.VisualControl != null)
            {
                SetControlValue(keyControl.VisualControl.Control, key);
                SetControlValue(valueControl.VisualControl.Control, value);

                Button removeKeyEntryButton = new() { Text = "-" };
                HBoxContainer hbox = new();

                removeKeyEntryButton.Pressed += () =>
                {
                    dictionaryVBox.RemoveChild(hbox);
                    dictionary.Remove(key);
                    valueChanged(dictionary);
                };

                hbox.AddChild(keyControl.VisualControl.Control);
                hbox.AddChild(valueControl.VisualControl.Control);
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
                SetControlValue(valueControl.VisualControl.Control, defaultValue);
            });

            if (keyControl.VisualControl != null && valueControl.VisualControl != null)
            {
                Button removeKeyEntryButton = new() { Text = "-" };
                HBoxContainer hbox = new();

                removeKeyEntryButton.Pressed += () =>
                {
                    dictionaryVBox.RemoveChild(hbox);
                    dictionary.Remove(oldKey);
                    valueChanged(dictionary);
                };

                hbox.AddChild(keyControl.VisualControl.Control);
                hbox.AddChild(valueControl.VisualControl.Control);
                hbox.AddChild(removeKeyEntryButton);
                dictionaryVBox.AddChild(hbox);
                dictionaryVBox.MoveChild(addButton, dictionaryVBox.GetChildCount() - 1);
            }
        }

        addButton.Pressed += AddNewEntryToDictionary;
        dictionaryVBox.AddChild(addButton);

        return new VisualControlInfo(new VBoxContainerControl(dictionaryVBox));
    }
}
