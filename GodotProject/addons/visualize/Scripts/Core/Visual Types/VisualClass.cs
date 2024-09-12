using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Visualize.Utils;

namespace Visualize.Core;

public static partial class VisualControlTypes
{
    private static VisualControlInfo VisualClass(object target, Type type, List<VisualSpinBox> debugExportSpinBoxes, Action<object> valueChanged)
    {
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

            if (control.VisualControl != null)
            {
                HBoxContainer hbox = new();
                hbox.AddChild(new Label { Text = property.Name.ToPascalCase().AddSpaceBeforeEachCapital() });
                hbox.AddChild(control.VisualControl.Control);
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

            if (control.VisualControl != null)
            {
                if (control.VisualControl.Control is SpinBox spinBox)
                {
                    spinBox.Editable = !field.IsLiteral;
                }
                else if (control.VisualControl.Control is LineEdit lineEdit)
                {
                    lineEdit.Editable = !field.IsLiteral;
                }
                else if (control.VisualControl.Control is BaseButton baseButton)
                {
                    baseButton.Disabled = field.IsLiteral;
                }

                HBoxContainer hbox = new();
                hbox.AddChild(new Label { Text = field.Name.ToPascalCase().AddSpaceBeforeEachCapital() });
                hbox.AddChild(control.VisualControl.Control);
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

        return new VisualControlInfo(new VBoxContainerControl(vbox));
    }
}
