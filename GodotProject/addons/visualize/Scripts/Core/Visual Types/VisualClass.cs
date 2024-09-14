using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Visualize.Utils;

namespace Visualize.Core;

public static partial class VisualControlTypes
{
    private static VisualControlInfo VisualClass(Type type, VisualControlContext context)
    {
        VBoxContainer vbox = new();

        List<IVisualControl> propertyControls = null;
        List<IVisualControl> fieldControls = null;

        if (context.InitialValue != null)
        {
            propertyControls = AddProperties(vbox, type, context);
            fieldControls = AddFields(vbox, type, context);
            AddMethods(vbox, type, context);
        }

        return new VisualControlInfo(new ClassControl(vbox, propertyControls, fieldControls));
    }

    private static List<IVisualControl> AddProperties(VBoxContainer vbox, Type type, VisualControlContext context)
    {
        List<IVisualControl> visualControls = new();

        BindingFlags flags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance | BindingFlags.DeclaredOnly;

        PropertyInfo[] properties = type.GetProperties(flags);

        foreach (PropertyInfo property in properties)
        {
            object initialValue = property.GetValue(context.InitialValue);

            MethodInfo propertySetMethod = property.GetSetMethod(true);

            VisualControlInfo control = CreateControlForType(property.PropertyType, new VisualControlContext(context.SpinBoxes, initialValue, v =>
            {
                property.SetValue(context.InitialValue, v);
                context.ValueChanged(context.InitialValue);
            }));

            if (control.VisualControl != null)
            {
                visualControls.Add(control.VisualControl);

                control.VisualControl.SetEditable(propertySetMethod != null);

                vbox.AddChild(CreateHBoxForMember(property.Name, control.VisualControl.Control));
            }
        }

        return visualControls;
    }

    private static List<IVisualControl> AddFields(VBoxContainer vbox, Type type, VisualControlContext context)
    {
        List<IVisualControl> visualControls = new();

        BindingFlags flags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance | BindingFlags.DeclaredOnly;

        FieldInfo[] fields = type
            .GetFields(flags)
            .Where(f => !f.Name.StartsWith("<") || !f.Name.EndsWith(">k__BackingField"))
            .ToArray();

        foreach (FieldInfo field in fields)
        {
            object initialValue = field.GetValue(context.InitialValue);

            VisualControlInfo control = CreateControlForType(field.FieldType, new VisualControlContext(context.SpinBoxes, initialValue, v =>
            {
                field.SetValue(context.InitialValue, v);
                context.ValueChanged(context.InitialValue);
            }));

            if (control.VisualControl != null)
            {
                visualControls.Add(control.VisualControl);

                control.VisualControl.SetEditable(!field.IsLiteral);

                vbox.AddChild(CreateHBoxForMember(field.Name, control.VisualControl.Control));
            }
        }

        return visualControls;
    }

    private static void AddMethods(VBoxContainer vbox, Type type, VisualControlContext context)
    {
        BindingFlags flags = BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.DeclaredOnly;

        // Cannot include private methods or else we will see Godots built in methods
        MethodInfo[] methods = type
            .GetMethods(flags)
            .Where(m => !m.Name.StartsWith("get_") && !m.Name.StartsWith("set_")).ToArray();

        foreach (MethodInfo method in methods)
        {
            ParameterInfo[] paramInfos = method.GetParameters();
            object[] providedValues = new object[paramInfos.Length];

            HBoxContainer hboxParams = VisualMethods.CreateMethodParameterControls(method, context.SpinBoxes, providedValues);
            Button button = VisualMethods.CreateMethodButton(method, context.InitialValue, paramInfos, providedValues);

            vbox.AddChild(hboxParams);
            vbox.AddChild(button);
        }
    }

    private static HBoxContainer CreateHBoxForMember(string memberName, Control control)
    {
        HBoxContainer hbox = new();
        hbox.AddChild(new Label { Text = memberName.ToPascalCase().AddSpaceBeforeEachCapital() });
        hbox.AddChild(control);
        return hbox;
    }
}

public class ClassControl : IVisualControl
{
    private readonly VBoxContainer _vboxContainer;
    private readonly List<IVisualControl> _visualPropertyControls;
    private readonly List<IVisualControl> _visualFieldControls;

    public ClassControl(VBoxContainer vboxContainer, List<IVisualControl> visualPropertyControls, List<IVisualControl> visualFieldControls)
    {
        _vboxContainer = vboxContainer;
        _visualPropertyControls = visualPropertyControls;
        _visualFieldControls = visualFieldControls;
    }

    public void SetValue(object value)
    {
        Type type = value.GetType();

        BindingFlags flags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance | BindingFlags.DeclaredOnly;

        PropertyInfo[] properties = type.GetProperties(flags);

        for (int i = 0; i < properties.Length; i++)
        {
            object propValue = properties[i].GetValue(value);
            _visualPropertyControls[i].SetValue(propValue);
        }

        FieldInfo[] fields = type
            .GetFields(flags)
            .Where(f => !f.Name.StartsWith("<") || !f.Name.EndsWith(">k__BackingField"))
            .ToArray();

        for (int i = 0; i < fields.Length; i++)
        {
            object fieldValue = fields[i].GetValue(value);
            _visualFieldControls[i].SetValue(fieldValue);
        }
    }

    public Control Control => _vboxContainer;

    public void SetEditable(bool editable)
    {
        foreach (IVisualControl visualControl in _visualPropertyControls)
        {
            visualControl.SetEditable(editable);
        }

        foreach (IVisualControl visualControl in _visualFieldControls)
        {
            visualControl.SetEditable(editable);
        }
    }
}
