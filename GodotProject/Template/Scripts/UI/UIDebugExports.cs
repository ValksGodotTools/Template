using Godot;
using System.Collections.Generic;

namespace Template;

public partial class UIDebugExports : Control
{
    [Export] VBoxContainer controlPanel;

    public override void _Ready()
    {
        List<DebugVisualNode> debugExportNodes = VisualizeAttributeHandler.RetrieveData(GetTree().Root);

        if (debugExportNodes.Count == 0)
            return;
        
        List<DebugVisualSpinBox> debugExportSpinBoxes = [];

        VisualUI.CreateVisualUIs(debugExportNodes, debugExportSpinBoxes);
        VisualUI.CreateStepPrecisionUI(debugExportSpinBoxes, controlPanel, GetTree());
    }
}
