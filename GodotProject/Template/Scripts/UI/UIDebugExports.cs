using System.Reflection;

namespace Template;

public class DebugExportNode
{
    public List<Node> Nodes { get; set; }
    public List<MemberInfo> ExportedMembers { get; set; }
}

public class DebugExportSpinBox
{
    public SpinBox SpinBox { get; set; }
    public Type Type { get; set; }
}

public partial class UIDebugExports : Control
{
    [Export] VBoxContainer vbox;

    public override void _Ready()
    {
        List<MemberInfo> exportedMembers = [];

        // Get all types in the current assembly
        Type[] types = Assembly.GetExecutingAssembly().GetTypes();

        List<DebugExportNode> debugExportNodes = types
            .Where(t => t.GetCustomAttributes(typeof(DebugExportsAttribute), false)
            .Any())
            .Select(type =>
            {
                DebugExportNode debugExportNode = new()
                {
                    Nodes = GetNodes(type),
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

        List<DebugExportSpinBox> debugExportSpinBoxes = [];

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
                if (IsWholeNumber(debugExportSpinBox.Type))
                {
                    double rounded = Mathf.RoundToInt(value);

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
        stepPrecision.Value = 0.1;

        foreach (DebugExportNode debugExportNode in debugExportNodes)
        {
            foreach (Node node in debugExportNode.Nodes)
            {
                foreach (MemberInfo member in debugExportNode.ExportedMembers)
                {
                    object value = null;

                    if (member is FieldInfo fieldInfo)
                    {
                        value = fieldInfo.GetValue(node);
                    }
                    else if (member is PropertyInfo propertyInfo)
                    {
                        value = propertyInfo.GetValue(node);
                    }

                    if (IsNumericType(value))
                    {
                        HBoxContainer hbox = new();

                        Label spinLabel = new()
                        {
                            Text = member.Name
                        };

                        hbox.AddChild(spinLabel);

                        SpinBox spinBox = new()
                        {
                            UpdateOnTextChanged = false,
                            AllowLesser = true,
                            AllowGreater = true
                        };

                        SetSpinBoxStep(spinBox, member, node);

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

                        vbox.AddChild(hbox);
                    }
                }
            }
        }
    }

    void SetSpinBoxStep(SpinBox spinBox, MemberInfo member, object yourObject)
    {
        object value = null;

        if (member is FieldInfo fieldInfo)
        {
            value = fieldInfo.GetValue(yourObject);
        }
        else if (member is PropertyInfo propertyInfo)
        {
            value = propertyInfo.GetValue(yourObject);
        }

        if (value != null)
        {
            Type valueType = value.GetType();

            double step = 0.1;
            double spinValue = Convert.ToDouble(value);

            if (valueType == typeof(int) || valueType == typeof(long) ||
                valueType == typeof(uint) || valueType == typeof(byte) ||
                valueType == typeof(sbyte) || valueType == typeof(short) ||
                valueType == typeof(ushort))
            {
                step = 1;
            }

            spinBox.Step = step;
            spinBox.Value = spinValue;
        }
    }

    public List<Node> GetNodes(Type type)
    {
        List<Node> nodes = [];
        RecursiveSearch(GetTree().Root, type, nodes);
        return nodes;
    }

    private void RecursiveSearch(Node node, Type type, List<Node> nodes)
    {
        if (node.GetType() == type)
        {
            nodes.Add(node);
        }

        foreach (Node child in node.GetChildren())
        {
            RecursiveSearch(child, type, nodes);
        }
    }

    static bool IsNumericType(object o)
    {
        if (o == null)
        {
            return false;
        }

        Type type = o.GetType();
        HashSet<Type> numericTypes =
        [
            typeof(int),
            typeof(float),
            typeof(double),
            typeof(long),
            typeof(short),
            typeof(ushort),
            typeof(uint),
            typeof(ulong),
            typeof(decimal),
            typeof(byte),
            typeof(sbyte)
        ];

        return numericTypes.Contains(type);
    }

    static bool IsWholeNumber(Type type)
    {
        return 
            type == typeof(int) ||
            type == typeof(long) ||
            type == typeof(short) ||
            type == typeof(byte) ||
            type == typeof(sbyte) ||
            type == typeof(uint) ||
            type == typeof(ulong) ||
            type == typeof(ushort);
    }

    private void SetMemberValue(MemberInfo member, object target, double value)
    {
        try
        {
            if (member is PropertyInfo property)
            {
                // Ensure the property is writeable
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
