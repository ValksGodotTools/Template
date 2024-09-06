using Godot;
using System;
using System.Reflection;

namespace Template;

public static class VisualNodeHandler
{
    public static void SetMemberValue(MemberInfo member, object target, object value)
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

    public static T GetMemberValue<T>(MemberInfo member, object node)
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

    public static object GetMemberValue(MemberInfo member, Node node)
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

    public static Type GetMemberType(MemberInfo member)
    {
        return member switch
        {
            FieldInfo fieldInfo => fieldInfo.FieldType,
            PropertyInfo propertyInfo => propertyInfo.PropertyType,
            _ => throw new ArgumentException("Member must be a field or property.")
        };
    }
}
