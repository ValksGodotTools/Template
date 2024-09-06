using Godot;
using System.Collections.Generic;

namespace Template;

public partial class UIVisualizeAttribute : Control
{
    [Export] VBoxContainer controlPanel;

    public override void _Ready()
    {
        List<DebugVisualNode> debugExportNodes = VisualizeAttributeHandler.RetrieveData(GetTree().Root);

        if (debugExportNodes.Count == 0)
            return;
        
        List<DebugVisualSpinBox> debugExportSpinBoxes = [];

        VisualUIBuilder.CreateVisualPanels(debugExportNodes, debugExportSpinBoxes);
        VisualUIBuilder.CreateStepPrecisionUI(debugExportSpinBoxes, controlPanel, GetTree());
    }
}
