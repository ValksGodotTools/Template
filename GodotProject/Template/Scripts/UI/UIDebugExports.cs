using CSharpUtils;
using Godot;
using GodotUtils;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System;

namespace Template;

public partial class UIDebugExports : Control
{
    [Export] VBoxContainer controlPanel;

    private const float INFO_PANEL_SCALE_FACTOR = 0.6f;

    public override void _Ready()
    {
        List<DebugVisualNode> debugExportNodes = GetVisualizedNodes(GetTree().Root);

        if (debugExportNodes.Count == 0)
            return;
        
        List<DebugVisualSpinBox> debugExportSpinBoxes = [];

        CreateVisualUIs(debugExportNodes, debugExportSpinBoxes);

        CreateStepPrecisionUI(debugExportSpinBoxes);
    }

    private static void CreateVisualUIs(List<DebugVisualNode> debugVisualNodes, List<DebugVisualSpinBox> debugExportSpinBoxes)
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
                            OptionButton optionButton = new();

                            // Add enum values to the OptionButton
                            foreach (object enumValue in Enum.GetValues(paramType))
                            {
                                optionButton.AddItem(enumValue.ToString());
                            }

                            optionButton.ItemSelected += item =>
                            {
                                object selectedValue = Enum.GetValues(paramType).GetValue(item);
                                providedValues[index] = selectedValue;
                                optionButton.ReleaseFocus();
                            };

                            hboxParams.AddChild(optionButton);
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
                            ColorPickerButton colorPickerButton = new()
                            {
                                CustomMinimumSize = Vector2.One * 30
                            };

                            colorPickerButton.ColorChanged += color =>
                            {
                                providedValues[index] = color;
                            };

                            colorPickerButton.PickerCreated += () =>
                            {
                                ColorPicker picker = colorPickerButton.GetPicker();

                                PopupPanel popupPanel = picker.GetParent<PopupPanel>();

                                popupPanel.InitialPosition = Window.WindowInitialPosition.Absolute;

                                popupPanel.AboutToPopup += () =>
                                {
                                    Vector2 viewportSize = node.GetTree().Root.GetViewport().GetVisibleRect().Size;

                                    popupPanel.Position = new Vector2I(
                                        (int)(viewportSize.X - popupPanel.Size.X),
                                        0);
                                };
                            };

                            colorPickerButton.PopupClosed += colorPickerButton.ReleaseFocus;

                            hboxParams.AddChild(colorPickerButton);
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

        Type type = GetMemberType(member);

        if (type.IsNumericType())
        {
            SpinBox spinBox = SpinBox(debugExportSpinBoxes, type);

            SetSpinBoxStepAndValue(spinBox, member, node);

            spinBox.ValueChanged += value =>
            {
                SetMemberValue(member, node, value);
            };

            element = spinBox;
        }
        else if (type == typeof(bool))
        {
            CheckBox checkBox = new();

            checkBox.ButtonPressed = GetMemberValue<bool>(member, node);

            checkBox.Toggled += value =>
            {
                SetMemberValue(member, node, value);
            };

            element = checkBox;
        }
        else if (type == typeof(Godot.Color))
        {
            element = CreateColorPicker(member, node);
        }
        else if (type == typeof(string))
        {
            element = CreateLineEdit(member, node);
        }
        else if (type.IsEnum)
        {
            element = CreateOptionButton(member, node);
        }
        else
        {
            throw new NotImplementedException($"The type '{type}' is not yet supported for the {nameof(VisualizeAttribute)}");
        }

        hbox.AddChild(element);

        return hbox;
    }

    private static OptionButton CreateOptionButton(MemberInfo member, Node node)
    {
        OptionButton optionButton = new();

        Type enumType = GetMemberType(member);

        // Add enum values to the OptionButton
        foreach (object enumValue in Enum.GetValues(enumType))
        {
            optionButton.AddItem(enumValue.ToString());
        }

        // Select the current value of the member
        object currentValue = GetMemberValue(member, node);
        int selectedIndex = Array.IndexOf(Enum.GetValues(enumType), currentValue);
        optionButton.Select(selectedIndex);

        optionButton.ItemSelected += item =>
        {
            object selectedValue = Enum.GetValues(enumType).GetValue(item);
            SetMemberValue(member, node, selectedValue);
        };

        return optionButton;
    }

    private static LineEdit CreateLineEdit(MemberInfo member, Node node)
    {
        LineEdit lineEdit = new();

        lineEdit.Text = GetMemberValue<string>(member, node);

        lineEdit.TextChanged += text =>
        {
            SetMemberValue(member, node, text);
        };

        return lineEdit;
    }

    private static ColorPickerButton CreateColorPicker(MemberInfo member, Node node)
    {
        ColorPickerButton colorPickerButton = new()
        {
            CustomMinimumSize = Vector2.One * 30
        };

        colorPickerButton.ColorChanged += color =>
        {
            SetMemberValue(member, node, color);
        };

        colorPickerButton.PickerCreated += () =>
        {
            ColorPicker picker = colorPickerButton.GetPicker();

            picker.Color = GetMemberValue<Color>(member, node);

            PopupPanel popupPanel = picker.GetParent<PopupPanel>();

            popupPanel.InitialPosition = Window.WindowInitialPosition.Absolute;

            popupPanel.AboutToPopup += () =>
            {
                Vector2 viewportSize = node.GetTree().Root.GetViewport().GetVisibleRect().Size;

                popupPanel.Position = new Vector2I(
                    (int)(viewportSize.X - popupPanel.Size.X),
                    0);
            };
        };

        colorPickerButton.PopupClosed += colorPickerButton.ReleaseFocus;

        return colorPickerButton;
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

    private void CreateStepPrecisionUI(List<DebugVisualSpinBox> debugExportSpinBoxes)
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
        unfocus.Pressed += () => GetTree().UnfocusCurrentControl();
        controlPanel.AddChild(unfocus);
    }

    private static OptionButton CreateStepPrecisionOptionButton(List<DebugVisualSpinBox> debugExportSpinBoxes)
    {
        OptionButton optionButton = new()
        {
            Alignment = HorizontalAlignment.Center
        };

        optionButton.AddItem("10");
        optionButton.AddItem("1");
        optionButton.AddItem("0.1");
        optionButton.AddItem("0.01");
        optionButton.AddItem("0.001");

        // Default precision will be 0.1
        optionButton.Select(2);

        optionButton.ItemSelected += itemId =>
        {
            foreach (DebugVisualSpinBox debugExportSpinBox in debugExportSpinBoxes)
            {
                double precision = Convert.ToDouble(optionButton.GetItemText((int)itemId));

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

        return optionButton;
    }

    private static List<DebugVisualNode> GetVisualizedNodes(Node parent)
    {
        List<DebugVisualNode> debugVisualNodes = [];

        Type[] types = Assembly.GetExecutingAssembly().GetTypes();

        IEnumerable<Type> visualTypes = HasVisualizeAttribute(types);

        BindingFlags flags = 
            BindingFlags.Public |
            BindingFlags.NonPublic |
            BindingFlags.Instance |
            BindingFlags.Static;

        foreach (Type type in types)
        {
            Vector2 initialPosition = Vector2.Zero;
            VisualizeAttribute attribute = (VisualizeAttribute)type.GetCustomAttribute(typeof(VisualizeAttribute), false);

            if (attribute != null)
            {
                initialPosition = attribute.InitialPosition;
            }

            List<Node> nodes = parent.GetNodes(type);

            foreach (Node node in nodes)
            {
                List<PropertyInfo> properties = [];
                List<FieldInfo> fields = [];
                List<MethodInfo> methods = [];

                foreach (PropertyInfo property in type.GetProperties(flags))
                {
                    if (property.GetCustomAttributes(typeof(VisualizeAttribute), false).Any())
                    {
                        properties.Add(property);
                    }
                }

                foreach (FieldInfo field in type.GetFields(flags))
                {
                    if (field.GetCustomAttributes(typeof(VisualizeAttribute), false).Any())
                    {
                        fields.Add(field);
                    }
                }

                foreach (MethodInfo method in type.GetMethods(flags))
                {
                    if (method.GetCustomAttributes(typeof(VisualizeAttribute), false).Any())
                    {
                        methods.Add(method);
                    }
                }

                if (properties.Any() || fields.Any() || methods.Any())
                {
                    debugVisualNodes.Add(new DebugVisualNode(node, initialPosition, properties, fields, methods));
                }
            }
        }

        return debugVisualNodes;
    }

    private static IEnumerable<T> HasVisualizeAttribute<T>(IEnumerable<T> collection) where T : MemberInfo
    {
        return collection
            .Where(x => x.GetCustomAttributes(typeof(VisualizeAttribute), false)
            .Any());
    }

    private static void SetSpinBoxStepAndValue(SpinBox spinBox, MemberInfo member, object instance)
    {
        double value = GetMemberValue<double>(member, instance);

        Type valueType = value.GetType();

        double step = 0.1;

        if (valueType.IsWholeNumber())
        {
            step = 1;
        }

        spinBox.Step = step;
        spinBox.Value = value;
    }

    private static void SetMemberValue(MemberInfo member, object target, object value)
    {
        try
        {
            // Set the value of the property or field
            if (member is PropertyInfo property)
            {
                if (property.CanWrite)
                {
                    if (property.GetMethod.IsStatic)
                    {
                        property.SetValue(null, Convert.ChangeType(value, property.PropertyType));
                    }
                    else
                    {
                        property.SetValue(target, Convert.ChangeType(value, property.PropertyType));
                    }
                }
                else
                {
                    GD.Print($"Property {member.Name} is read-only.");
                }
            }
            else if (member is FieldInfo field)
            {
                if (field.IsStatic)
                {
                    field.SetValue(null, Convert.ChangeType(value, field.FieldType));
                }
                else
                {
                    field.SetValue(target, Convert.ChangeType(value, field.FieldType));
                }
            }
        }
        catch (Exception ex)
        {
            GD.Print($"Failed to set value for {member.Name}: {ex.Message}");
        }
    }

    private static T GetMemberValue<T>(MemberInfo member, object node)
    {
        object value = member switch
        {
            FieldInfo fieldInfo when fieldInfo.IsStatic => fieldInfo.GetValue(null),
            FieldInfo fieldInfo => fieldInfo.GetValue(node),

            PropertyInfo propertyInfo when propertyInfo.GetMethod.IsStatic => propertyInfo.GetValue(null),
            PropertyInfo propertyInfo => propertyInfo.GetValue(node),

            _ => throw new ArgumentException("Member is not a FieldInfo or PropertyInfo")
        };

        if (value is float floatValue && typeof(T) == typeof(double))
        {
            return (T)(object)Convert.ToDouble(floatValue);
        }

        return (T)value;
    }

    private static object GetMemberValue(MemberInfo member, Node node)
    {
        return member switch
        {
            FieldInfo fieldInfo when fieldInfo.IsStatic => fieldInfo.GetValue(null),
            FieldInfo fieldInfo => fieldInfo.GetValue(node),

            PropertyInfo propertyInfo when propertyInfo.GetMethod.IsStatic => propertyInfo.GetValue(null),
            PropertyInfo propertyInfo => propertyInfo.GetValue(node),

            _ => throw new ArgumentException("Member must be a field or property.")
        };
    }

    private static Type GetMemberType(MemberInfo member)
    {
        return member switch
        {
            FieldInfo fieldInfo => fieldInfo.FieldType,
            PropertyInfo propertyInfo => propertyInfo.PropertyType,
            _ => throw new ArgumentException("Member must be a field or property.")
        };
    }
}

public class DebugVisualNode(Node node, Vector2 initialPosition, IEnumerable<PropertyInfo> properties, IEnumerable<FieldInfo> fields, IEnumerable<MethodInfo> methods)
{
    public Node Node { get; } = node;
    public Vector2 InitialPosition { get; } = initialPosition;
    public IEnumerable<PropertyInfo> Properties { get; } = properties;
    public IEnumerable<FieldInfo> Fields { get; } = fields;
    public IEnumerable<MethodInfo> Methods { get; } = methods;
}

public class DebugVisualSpinBox
{
    public SpinBox SpinBox { get; set; }
    public Type Type { get; set; }
}

public class ParameterConverter
{
    public static object[] ConvertParameterInfoToObjectArray(ParameterInfo[] paramInfos, object[] providedValues)
    {
        if (paramInfos == null)
        {
            throw new ArgumentNullException(nameof(paramInfos));
        }

        if (providedValues == null)
        {
            throw new ArgumentNullException(nameof(providedValues));
        }

        if (paramInfos.Length != providedValues.Length)
        {
            throw new ArgumentException("The number of provided values does not match the number of method parameters.");
        }

        object[] parameters = new object[paramInfos.Length];

        for (int i = 0; i < paramInfos.Length; i++)
        {
            ParameterInfo paramInfo = paramInfos[i];
            object providedValue = providedValues[i];

            if (providedValue == null)
            {
                if (paramInfo.ParameterType == typeof(string))
                {
                    parameters[i] = null;
                }
                else if (paramInfo.ParameterType.IsValueType)
                {
                    // Assign default value for value types
                    parameters[i] = Activator.CreateInstance(paramInfo.ParameterType);
                }
                else
                {
                    parameters[i] = null;
                }
            }
            else
            {
                if (!paramInfo.ParameterType.IsAssignableFrom(providedValue.GetType()))
                {
                    throw new InvalidOperationException($"The provided value for parameter '{paramInfo.Name}' is not assignable to the parameter type '{paramInfo.ParameterType}'.");
                }

                parameters[i] = providedValue;
            }
        }

        return parameters;
    }
}

