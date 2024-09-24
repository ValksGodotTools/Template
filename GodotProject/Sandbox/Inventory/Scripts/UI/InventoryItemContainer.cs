using Godot;
using GodotUtils;
using System;

namespace Template.Inventory;

public class InventoryItemContainer
{
    /// <summary>
    /// Occurs when the mouse cursor enters this container.
    /// </summary>
    public event Action<ItemContainerMouseEventArgs> MouseEntered;

    /// <summary>
    /// Occurs when the mouse cursor exits this container.
    /// </summary>
    public event Action<ItemContainerMouseEventArgs> MouseExited;

    public DraggableItem DraggableItem { get; private set; }
    public Control ItemParent { get; private set; }
    public InventoryContainer InventoryContainer { get; private set; }
    public Item Item { get; set; }

    public InventoryItemContainer(int index, float size, Node parent, InventoryContainer inventoryContainer)
    {
        InventoryContainer = inventoryContainer;

        PanelContainer container = new()
        {
            CustomMinimumSize = Vector2.One * size
        };

        container.MouseEntered += () =>
        {
            MouseEntered(new ItemContainerMouseEventArgs(index, this));
        };

        container.MouseExited += () =>
        {
            MouseExited(new ItemContainerMouseEventArgs(index, this));
        };

        ItemParent = AddCenterItemContainer(container);

        parent.AddChild(container);
    }

    public void SetItemSprite(InventoryItemSprite sprite)
    {
        DraggableItem = sprite.DraggableItem;
        ItemParent.QueueFreeChildren();
        ItemParent.AddChild(sprite.Build());
    }

    public void SetItemCount(int count)
    {
        DraggableItem.SetItemCount(count);
    }

    private Control AddCenterItemContainer(PanelContainer container)
    {
        CenterContainer center = new();
        Control control = new();

        container.AddChild(center);
        center.AddChild(control);

        return control;
    }
}
