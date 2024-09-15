using Godot;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace Template;

public class VisualNode(Node node, Vector2 initialPosition, string[] visualizeMembers, IEnumerable<PropertyInfo> properties, IEnumerable<FieldInfo> fields, IEnumerable<MethodInfo> methods)
{
    public Node Node { get; } = node;
    public string[] VisualizeMembers { get; } = visualizeMembers;
    public Vector2 InitialPosition { get; } = initialPosition;
    public IEnumerable<PropertyInfo> Properties { get; } = properties;
    public IEnumerable<FieldInfo> Fields { get; } = fields;
    public IEnumerable<MethodInfo> Methods { get; } = methods;
}

public class VisualSpinBox
{
    public SpinBox SpinBox { get; set; }
    public Type Type { get; set; }
}
