using Godot;
using System;
using System.Collections.Generic;
using Visualize.Utils;

namespace Visualize.Core;

public static partial class VisualControlTypes
{
    public static VisualControlInfo CreateControlForType(object initialValue, Type type, List<VisualSpinBox> debugExportSpinBoxes, Action<object> valueChanged)
    {
        VisualControlInfo info = type switch
        {
            _ when type == typeof(bool) => VisualBool(initialValue, v => valueChanged(v)),
            _ when type == typeof(string) => VisualString(initialValue, v => valueChanged(v)),
            _ when type == typeof(object) => VisualObject(initialValue, v => valueChanged(v)),
            _ when type == typeof(Color) => VisualColor(initialValue, v => valueChanged(v)),
            _ when type == typeof(Vector2) => VisualVector2(initialValue, v => valueChanged(v)),
            _ when type == typeof(Vector2I) => VisualVector2I(initialValue, v => valueChanged(v)),
            _ when type == typeof(Vector3) => VisualVector3(initialValue, v => valueChanged(v)),
            _ when type == typeof(Vector3I) => VisualVector3I(initialValue, v => valueChanged(v)),
            _ when type == typeof(Vector4) => VisualVector4(initialValue, v => valueChanged(v)),
            _ when type == typeof(Vector4I) => VisualVector4I(initialValue, v => valueChanged(v)),
            _ when type == typeof(Quaternion) => VisualQuaternion(initialValue, v => valueChanged(v)),
            _ when type == typeof(NodePath) => VisualNodePath(initialValue, v => valueChanged(v)),
            _ when type == typeof(StringName) => VisualStringName(initialValue, v => valueChanged(v)),
            _ when type.IsNumericType() => VisualNumeric(initialValue, type, debugExportSpinBoxes, v => valueChanged(v)),
            _ when type.IsEnum => VisualEnum(initialValue, type, v => valueChanged(v)),
            _ when type.IsArray => VisualArray(initialValue, type, debugExportSpinBoxes, v => valueChanged(v)),
            _ when type.IsGenericType && type.GetGenericTypeDefinition() == typeof(List<>) => VisualList(initialValue, type, debugExportSpinBoxes, v => valueChanged(v)),
            _ when type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Dictionary<,>) => VisualDictionary(initialValue, type, debugExportSpinBoxes, v => valueChanged(v)),
            //_ when type.IsClass && !type.IsSubclassOf(typeof(GodotObject)) => VisualClass(initialValue, type, debugExportSpinBoxes, v => valueChanged(v)),
            //_ when type.IsValueType && !type.IsClass && !type.IsSubclassOf(typeof(GodotObject)) => VisualClass(initialValue, type, debugExportSpinBoxes, v => valueChanged(v)),
            _ => new VisualControlInfo(null)
        };

        if (info.VisualControl == null)
        {
            PrintUtils.Warning($"The type '{type.Namespace}.{type.Name}' is not supported for the {nameof(VisualizeAttribute)}");
        }

        return info;
    }
}

public interface IVisualControl
{
    void SetValue(object value);
    Control Control { get; }
    void SetEditable(bool editable);
}

public class VisualControlInfo
{
    public IVisualControl VisualControl { get; }

    public VisualControlInfo(IVisualControl visualControl)
    {
        VisualControl = visualControl;
    }
}
