using Godot;
using System.Collections.Generic;

namespace Template;

public partial class UIVisualizeAttribute : Control
{
    public override void _Ready()
    {
        List<DebugVisualNode> visualAttributeData = VisualizeAttributeHandler.RetrieveData(GetTree().Root);

        if (visualAttributeData.Count > 0)
        {
            List<DebugVisualSpinBox> debugExportSpinBoxes = [];

            VisualUIBuilder.CreateVisualPanels(visualAttributeData, debugExportSpinBoxes);
        }
    }
}
