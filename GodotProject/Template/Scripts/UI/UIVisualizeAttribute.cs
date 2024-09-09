using Godot;
using System.Collections.Generic;

namespace Template;

public partial class UIVisualizeAttribute : Control
{
    public override void _Ready()
    {
        List<VisualNode> visualAttributeData = VisualizeAttributeHandler.RetrieveData(GetTree().Root);

        if (visualAttributeData.Count > 0)
        {
            List<VisualSpinBox> debugExportSpinBoxes = [];

            VisualUI.CreateVisualPanels(visualAttributeData, debugExportSpinBoxes);
        }
    }
}
