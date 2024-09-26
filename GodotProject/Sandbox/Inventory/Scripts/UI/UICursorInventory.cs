using Godot;

namespace Template.Inventory;

public class UICursorInventory
{
    public CursorInventory Inventory { get; }
    public CursorItemContainer CursorItemContainer { get; }

    public UICursorInventory(Node parent)
    {
        CursorItemContainer = CursorItemContainer.Instantiate();

        parent.AddChild(CursorItemContainer);

        Inventory = new();
        Inventory.OnItemChanged += CursorItemContainer.SetItem;
    }
}
