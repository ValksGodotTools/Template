using System.Reflection;

namespace Template;

public partial class UIDebugExports : Control
{
    [Export] VBoxContainer controlPanel;

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
                //GD.Print(method.Name);
            }

            // All debug UI elements should not be influenced by the game world environments lighting
            vbox.GetChildren<Control>().ForEach(child => child.SetUnshaded());

            node.AddChild(vbox);
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
            element = CreateSpinBoxUI(member, type, node, debugExportSpinBoxes);
        }
        else if (type == typeof(bool))
        {
            element = CreateCheckBoxUI(member, node);
        }
        else if (type == typeof(Godot.Color))
        {
            element = CreateColorPicker(member, node);
        }
        else if (type == typeof(string))
        {
            element = CreateLineEdit(member, node);
        }
        else
        {
            throw new NotImplementedException($"The type '{type}' is not yet supported for the {nameof(VisualizeAttribute)}");
        }

        hbox.AddChild(element);

        return hbox;
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
        ColorPickerButton colorPickerButton = new();

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

    private static CheckBox CreateCheckBoxUI(MemberInfo member, Node node)
    {
        CheckBox checkBox = new();
        
        checkBox.ButtonPressed = GetMemberValue<bool>(member, node);

        checkBox.Toggled += value =>
        {
            SetMemberValue(member, node, value);
        };

        return checkBox;
    }

    private static Control CreateSpinBoxUI(MemberInfo member, Type type, Node node, List<DebugVisualSpinBox> debugExportSpinBoxes)
    {
        // Create a SpinBox for numeric input
        SpinBox spinBox = new()
        {
            UpdateOnTextChanged = true,
            AllowLesser = true,
            AllowGreater = true,
            Alignment = HorizontalAlignment.Center
        };

        SetSpinBoxStepAndValue(spinBox, member, node);

        debugExportSpinBoxes.Add(new DebugVisualSpinBox
        {
            SpinBox = spinBox,
            Type = type
        });

        spinBox.ValueChanged += value =>
        {
            SetMemberValue(member, node, value);
        };

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
            BindingFlags.Public | // Public
            BindingFlags.NonPublic | // Private
            BindingFlags.Instance; // Instanced

        foreach (Type type in types)
        {
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
                    debugVisualNodes.Add(new DebugVisualNode(node, properties, fields, methods));
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
                    property.SetValue(target, Convert.ChangeType(value, property.PropertyType));
                }
                else
                {
                    GD.Print($"Property {member.Name} is read-only.");
                }
            }
            else if (member is FieldInfo field)
            {
                field.SetValue(target, Convert.ChangeType(value, field.FieldType));
            }
        }
        catch (Exception ex)
        {
            GD.Print($"Failed to set value for {member.Name}: {ex.Message}");
        }
    }

    private static T GetMemberValue<T>(MemberInfo member, object node)
    {
        if (member is FieldInfo fieldInfo)
        {
            return (T)fieldInfo.GetValue(node);
        }
        else if (member is PropertyInfo propertyInfo)
        {
            return (T)propertyInfo.GetValue(node);
        }
        throw new ArgumentException("Member is not a FieldInfo or PropertyInfo");
    }
}

public class DebugVisualNode(Node node, IEnumerable<PropertyInfo> properties, IEnumerable<FieldInfo> fields, IEnumerable<MethodInfo> methods)
{
    public Node Node { get; } = node;
    public IEnumerable<PropertyInfo> Properties { get; } = properties;
    public IEnumerable<FieldInfo> Fields { get; } = fields;
    public IEnumerable<MethodInfo> Methods { get; } = methods;
}

public class DebugVisualSpinBox
{
    public SpinBox SpinBox { get; set; }
    public Type Type { get; set; }
}
