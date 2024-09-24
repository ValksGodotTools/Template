using Godot;
using GodotUtils;
using System;

namespace Template.Inventory;

public class InventoryItemContainer
{
    public event Action<ItemContainerMouseEventArgs> MouseEntered;
    public event Action<ItemContainerMouseEventArgs> MouseExited;

    public UIItem UIItem { get; set; }
    public Control ItemParent { get; private set; }
    public InventoryContainer InventoryContainer { get; private set; }
    public int Index { get; private set; }
    public Item Item { get; set; }

    private const int PIXEL_SIZE = 50;

    public InventoryItemContainer(int index, Node parent, InventoryContainer inventoryContainer)
    {
        InventoryContainer = inventoryContainer;
        Index = index;

        PanelContainer container = new()
        {
            CustomMinimumSize = Vector2.One * PIXEL_SIZE
        };

        container.MouseEntered += () => MouseEntered(new ItemContainerMouseEventArgs(index, this));
        container.MouseExited += () => MouseExited(new ItemContainerMouseEventArgs(index, this));

        ItemParent = AddCenterItemContainer(container);
        parent.AddChild(container);
    }

    public void SetItem(Item item)
    {
        ItemParent.QueueFreeChildren();

        ItemVisualData itemVisualData = ItemSpriteManager.GetResource(item);
        InventoryItemSprite sprite = ResourceFactoryRegistry.CreateSprite(itemVisualData, this);
        UIItem = sprite.UIItem;

        ItemParent.AddChild(sprite.Build());
        UIItem.SetItemCount(item.Count);
        UIItem.SetInventoryItemContainer(this);
    }

    public void SwapItems(InventoryItemContainer other)
    {
        if (other == null)
            return;

        Inventory thisInventory = InventoryContainer.Inventory;
        Inventory otherInventory = other.InventoryContainer.Inventory;

        Item tempItem = thisInventory.GetItem(Index);
        thisInventory.SetItem(Index, otherInventory.GetItem(other.Index));
        otherInventory.SetItem(other.Index, tempItem);

        (other.UIItem, UIItem) = (UIItem, other.UIItem);

        if (UIItem != null)
            SetItem(thisInventory.GetItem(Index));

        if (other.UIItem != null)
            other.SetItem(otherInventory.GetItem(other.Index));
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
