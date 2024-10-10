using System;

namespace Template.Inventory;

public class InventoryActionEventArgs(InventoryAction action) : EventArgs
{
    public InventoryAction Action { get; } = action;

    // Specific to Transfer Actions
    public ItemContainer TargetItemContainer { get; set; }
    public int FromIndex { get; set; }
    public bool AreSameType { get; set; }
}
