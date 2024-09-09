using Godot;
using System.Collections.Generic;

namespace Visualize;

public partial class VisualizeAutoload : Node
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
