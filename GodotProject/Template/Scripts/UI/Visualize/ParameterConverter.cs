using System;
using System.Reflection;

namespace Template;

public static class ParameterConverter
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
