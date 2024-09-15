using System;
using System.Collections.Generic;
using System.Reflection;

namespace Visualize.Core;

public class VisualControlContext
{
    public List<VisualSpinBox> SpinBoxes { get; set; } = [];
    public object InitialValue { get; set; }
    public Action<object> ValueChanged { get; set; }

    public VisualControlContext(List<VisualSpinBox> spinBoxes, object initialValue, Action<object> valueChanged)
    {
        SpinBoxes = spinBoxes;
        InitialValue = initialValue;
        ValueChanged = valueChanged;
    }
}

