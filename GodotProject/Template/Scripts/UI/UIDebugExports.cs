using System.Reflection;

namespace Template;

public partial class UIDebugExports : Control
{
    // Reference to a VBoxContainer node in the scene
    [Export] VBoxContainer vbox;

    public override void _Ready()
    {
        List<DebugExportSpinBox> debugExportSpinBoxes = [];

        List<DebugExportNode> debugExportNodes = GetDebugExportNodes(GetTree().Root);

        List<MemberInfo> exportedMembers = [];

        CreateStepPrecisionUI(debugExportSpinBoxes);

        CreateMemberInfoUI(debugExportNodes, debugExportSpinBoxes, vbox);
    }

    // Method to create UI elements for member info
    private static void CreateMemberInfoUI(List<DebugExportNode> debugExportNodes, List<DebugExportSpinBox> debugExportSpinBoxes, Node parent)
    {
        foreach (DebugExportNode debugExportNode in debugExportNodes)
        {
            foreach (Node node in debugExportNode.Nodes)
            {
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
                            Text = member.Name
                        };

                        hbox.AddChild(spinLabel);

                        // Create a SpinBox for numeric input
                        SpinBox spinBox = new()
                        {
                            UpdateOnTextChanged = false,
                            AllowLesser = true,
                            AllowGreater = true
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
        }
    }

    private void CreateStepPrecisionUI(List<DebugExportSpinBox> debugExportSpinBoxes)
    {
        HBoxContainer hBoxContainer = new();

        Label label = new()
        {
            Text = "Step Precision"
        };

        SpinBox stepPrecision = new()
        {
            Step = 0.01
        };

        stepPrecision.ValueChanged += value =>
        {
            foreach (DebugExportSpinBox debugExportSpinBox in debugExportSpinBoxes)
            {
                if (debugExportSpinBox.Type.IsWholeNumber())
                {
                    double rounded = Mathf.RoundToInt(value);

                    // Ensure the rounded value is not zero
                    if (rounded == 0)
                    {
                        rounded = 1;
                    }

                    debugExportSpinBox.SpinBox.Step = rounded;
                }
                else
                {
                    debugExportSpinBox.SpinBox.Step = value;
                }
            }
        };

        hBoxContainer.AddChild(label);

        hBoxContainer.AddChild(stepPrecision);

        vbox.AddChild(hBoxContainer);

        // Set the initial value of the step precision SpinBox
        stepPrecision.Value = 0.1;
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
