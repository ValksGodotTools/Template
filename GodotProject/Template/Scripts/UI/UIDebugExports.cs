using System.Reflection;

namespace Template;

public partial class UIDebugExports : Control
{
    // Reference to a VBoxContainer node in the scene
    [Export] VBoxContainer sidePanelNodeInfoVBox;

    private List<VisualizedNode> visualizedNodes = [];

    public override void _Ready()
    {
        List<DebugExportNode> debugExportNodes = GetDebugExportNodes(GetTree().Root);

        if (debugExportNodes.Count == 0)
            return;

        List<DebugExportSpinBox> debugExportSpinBoxes = [];

        List<MemberInfo> exportedMembers = [];

        CreateMemberInfoUI(debugExportNodes.FirstOrDefault(), debugExportSpinBoxes, sidePanelNodeInfoVBox);

        CreateStepPrecisionUI(debugExportSpinBoxes);

        visualizedNodes = CreateVisualizationUI(debugExportNodes);
    }

    public override void _Process(double delta)
    {
        foreach (VisualizedNode node in visualizedNodes)
        {
            node.Process();
        }
    }

    private static List<VisualizedNode> CreateVisualizationUI(List<DebugExportNode> debugExportNodes)
    {
        List<VisualizedNode> visualizedNodes = [];

        foreach (DebugExportNode exportNode in debugExportNodes)
        {
            IEnumerable<MemberInfo> memberQuery = from member in exportNode.ExportedMembers
                where member.GetCustomAttributes(typeof(VisualizeAttribute)).Any()
                select member;

            List<MemberInfo> members = memberQuery.ToList();

            foreach (Node node in exportNode.Nodes)
            {
                List<(MemberInfo, Label)> memberLabels = [];

                VBoxContainer infoContainer = new();

                foreach (MemberInfo member in members)
                {
                    HBoxContainer memberContainer = new();

                    Label name = new() { Text = member.Name.ToPascalCase().AddSpaceBeforeEachCapital() };
                    Label value = new();

                    memberContainer.AddChild(name);
                    memberContainer.AddChild(value);

                    memberLabels.Add((member, name));
                    infoContainer.AddChild(memberContainer);
                }

                node.AddChild(infoContainer);

                VisualizedNode visualizedNode = new(node, memberLabels);
                visualizedNode.Process();

                visualizedNodes.Add(visualizedNode);
            }
        }

        return visualizedNodes;
    }

    // Method to create UI elements for member info
    private static void CreateMemberInfoUI(DebugExportNode debugExportNode, List<DebugExportSpinBox> debugExportSpinBoxes, VBoxContainer parent)
    {
        Node node = debugExportNode.Nodes.FirstOrDefault();

        parent.AddChild(new GLabel(node.Name));

        foreach (MemberInfo member in debugExportNode.ExportedMembers)
        {
            object value = null;

            // Get the value of the field or property
            if (member is FieldInfo fieldInfo)
            {
                value = fieldInfo.GetValue(node);
            }
            else if (member is PropertyInfo propertyInfo)
            {
                value = propertyInfo.GetValue(node);
            }

            // Create the UI for the member info
            if (value.IsNumericType())
            {
                HBoxContainer hbox = new();

                Label spinLabel = new()
                {
                    Text = member.Name.ToPascalCase().AddSpaceBeforeEachCapital(),
                    SizeFlagsHorizontal = SizeFlags.ExpandFill
                };

                hbox.AddChild(spinLabel);

                // Create a SpinBox for numeric input
                SpinBox spinBox = new()
                {
                    UpdateOnTextChanged = false,
                    AllowLesser = true,
                    AllowGreater = true,
                    Alignment = HorizontalAlignment.Center
                };

                SetSpinBoxStepAndValue(spinBox, member, node);

                debugExportSpinBoxes.Add(new DebugExportSpinBox
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
    }

    private void CreateStepPrecisionUI(List<DebugExportSpinBox> debugExportSpinBoxes)
    {
        HBoxContainer hbox = new();

        Label label = new()
        {
            Text = "Step Precision",
            SizeFlagsHorizontal = SizeFlags.ExpandFill
        };

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
            foreach (DebugExportSpinBox debugExportSpinBox in debugExportSpinBoxes)
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

        hbox.AddChild(label);
        hbox.AddChild(optionButton);

        sidePanelNodeInfoVBox.AddChild(hbox);
    }

    // Method to get DebugExportNodes from the scene tree
    private static List<DebugExportNode> GetDebugExportNodes(Node parent)
    {
        Type[] types = Assembly.GetExecutingAssembly().GetTypes();

        // Select types with the DebugExportsAttribute and create DebugExportNode instances
        List<DebugExportNode> debugExportNodes = types
            .Where(t => t.GetCustomAttributes(typeof(DebugExportsAttribute), false)
            .Any())
            .Select(type =>
            {
                DebugExportNode debugExportNode = new()
                {
                    Nodes = parent.GetNodes(type),
                    ExportedMembers = []
                };

                // Get all properties and fields with the [Export] attribute
                IEnumerable<PropertyInfo> properties = type.GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
                    .Where(p => p.GetCustomAttributes(typeof(ExportAttribute), false)
                    .Any());

                IEnumerable<FieldInfo> fields = type.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
                    .Where(f => f.GetCustomAttributes(typeof(ExportAttribute), false)
                    .Any());

                // Add them to the list
                debugExportNode.ExportedMembers.AddRange(properties);
                debugExportNode.ExportedMembers.AddRange(fields);

                return debugExportNode;
            })
            .ToList();

        return debugExportNodes;
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

// Class to store information about nodes and their exported members
public class DebugExportNode
{
    public List<Node> Nodes { get; set; }
    public List<MemberInfo> ExportedMembers { get; set; }
}

// Class to store information about SpinBoxes and their types
public class DebugExportSpinBox
{
    public SpinBox SpinBox { get; set; }
    public Type Type { get; set; }
}

public class VisualizedNode(Node node, List<(MemberInfo, Label)> members)
{
    public Node Node { get; } = node;
    public List<(MemberInfo, Label)> Members { get; } = members;

    public void Process()
    {
        foreach ((MemberInfo info, Label label) in Members)
        {
            label.Text = GetValue(info) ?? "<null>";
        }
    }

    private string GetValue(MemberInfo info)
    {
        if (info is FieldInfo fieldInfo)
        {
            return fieldInfo.GetValue(Node)?.ToString();
        }

        if (info is PropertyInfo propertyInfo)
        {
            return propertyInfo.GetValue(Node)?.ToString();
        }

        return null;
    }
}