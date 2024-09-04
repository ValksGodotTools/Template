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
            label.SetUnshaded();

            vbox.AddChild(label);

            foreach (PropertyInfo property in debugVisualNode.Properties)
            {
                object value = property.GetValue(node);

                CreateMemberInfoElement(property, value, node, vbox, debugExportSpinBoxes);
            }

            foreach (FieldInfo field in debugVisualNode.Fields)
            {
                object value = field.GetValue(node);

                CreateMemberInfoElement(field, value, node, vbox, debugExportSpinBoxes);
            }

            node.AddChild(vbox);
        }
    }

    private static void CreateMemberInfoElement(MemberInfo member, object value, Node node, Node parent, List<DebugVisualSpinBox> debugExportSpinBoxes)
    {
        // Create the UI for the member info
        if (value.IsNumericType())
        {
            HBoxContainer hbox = new();

            GLabel spinLabel = new()
            {
                Text = member.Name.ToPascalCase().AddSpaceBeforeEachCapital(),
                SizeFlagsHorizontal = SizeFlags.ExpandFill
            };

            spinLabel.SetUnshaded();

            hbox.AddChild(spinLabel);

            // Create a SpinBox for numeric input
            SpinBox spinBox = new()
            {
                UpdateOnTextChanged = true,
                AllowLesser = true,
                AllowGreater = true,
                Alignment = HorizontalAlignment.Center
            };

            spinBox.SetUnshaded();

            SetSpinBoxStepAndValue(spinBox, member, node);

            debugExportSpinBoxes.Add(new DebugVisualSpinBox
            {
                SpinBox = spinBox,
                Type = value.GetType()
            });

            spinBox.ValueChanged += value =>
            {
                SetMemberValue(member, node, value);
            };

            hbox.AddChild(spinBox);

            parent.AddChild(hbox);
        }
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
            BindingFlags.Instance | // Instanced
            BindingFlags.DeclaredOnly; // Exclude inherited members

        foreach (Type type in visualTypes)
        {
            List<Node> nodes = parent.GetNodes(type);

            foreach (Node node in nodes)
            {
                IEnumerable<PropertyInfo> properties = type.GetProperties(flags);
                IEnumerable<FieldInfo> fields = type.GetFields(flags);
                IEnumerable<MethodInfo> methods = type.GetMethods(flags);

                debugVisualNodes.Add(new DebugVisualNode(node, properties, fields, methods));
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
        object value = null;

        // Get the value of the field or property
        if (member is FieldInfo fieldInfo)
        {
            value = fieldInfo.GetValue(instance);
        }
        else if (member is PropertyInfo propertyInfo)
        {
            value = propertyInfo.GetValue(instance);
        }

        if (value != null)
        {
            Type valueType = value.GetType();

            double step = 0.1;
            double spinValue = Convert.ToDouble(value);

            if (valueType.IsWholeNumber())
            {
                step = 1;
            }

            spinBox.Step = step;
            spinBox.Value = spinValue;
        }
    }

    private static void SetMemberValue(MemberInfo member, object target, double value)
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
