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
        Initialize(index, inventoryContainer);
        CreateContainer(parent);
    }

    private void Initialize(int index, InventoryContainer inventoryContainer)
    {
        InventoryContainer = inventoryContainer;
        Index = index;
    }

    private void CreateContainer(Node parent)
    {
        PanelContainer container = CreatePanelContainer();
        ItemParent = AddCenterItemContainer(container);
        parent.AddChild(container);
    }

    private PanelContainer CreatePanelContainer()
    {
        PanelContainer container = new()
        {
            CustomMinimumSize = Vector2.One * PIXEL_SIZE
        };

        container.MouseEntered += OnMouseEntered;
        container.MouseExited += OnMouseExited;

        return container;
    }

    private void OnMouseEntered()
    {
        MouseEntered?.Invoke(new ItemContainerMouseEventArgs(Index, this));
    }

    private void OnMouseExited()
    {
        MouseExited?.Invoke(new ItemContainerMouseEventArgs(Index, this));
    }

    public void SetItem(Item item)
    {
        ClearItemParent();
        CreateItemSprite(item);
        UpdateUIItem(item);
    }

    public void ClearItemParent()
    {
        ItemParent.QueueFreeChildren();
        UIItem = null;
    }

    private void CreateItemSprite(Item item)
    {
        if (item == null)
            return;

        ItemVisualData itemVisualData = ItemSpriteManager.GetResource(item);
        InventoryItemSprite sprite = ResourceFactoryRegistry.CreateSprite(itemVisualData, this);
        UIItem = sprite.UIItem;
        ItemParent.AddChild(sprite.Build());
    }

    private void UpdateUIItem(Item item)
    {
        if (item == null)
            return;

        UIItem.SetItemCount(item.Count);
        UIItem.SetInventoryItemContainer(this);
    }

    public void SwapItems(InventoryItemContainer other)
    {
        if (other == null)
            return;

        SwapInventoryItems(other);
        SwapUIItems(other);
        UpdateItemsAfterSwap(other);
    }

    private void SwapInventoryItems(InventoryItemContainer other)
    {
        Inventory thisInventory = InventoryContainer.Inventory;
        Inventory otherInventory = other.InventoryContainer.Inventory;

        Item tempItem = thisInventory.GetItem(Index);
        thisInventory.SetItem(Index, otherInventory.GetItem(other.Index));
        otherInventory.SetItem(other.Index, tempItem);
    }

    private void SwapUIItems(InventoryItemContainer other)
    {
        (other.UIItem, UIItem) = (UIItem, other.UIItem);
    }

    private void UpdateItemsAfterSwap(InventoryItemContainer other)
    {
        if (UIItem != null)
            SetItem(InventoryContainer.Inventory.GetItem(Index));

        if (other.UIItem != null)
            other.SetItem(other.InventoryContainer.Inventory.GetItem(other.Index));
    }

    private Control AddCenterItemContainer(PanelContainer container)
    {
        CenterContainer center = new();
        Control control = new();

        container.AddChild(center);
        center.AddChild(control);

        return control;
    }

    public void CombineItems(InventoryItemContainer other)
    {
        if (other == null)
            return;

        Inventory thisInventory = InventoryContainer.Inventory;
        Inventory otherInventory = other.InventoryContainer.Inventory;

        Item thisItem = thisInventory.GetItem(Index);
        Item otherItem = otherInventory.GetItem(other.Index);

        if (otherItem != null && thisItem != null && otherItem.Equals(thisItem))
        {
            // Combine counts if items are of the same type
            otherItem.Count += thisItem.Count;
            otherInventory.SetItem(other.Index, otherItem); // Update the inventory
            other.SetItem(otherItem); // Update the UI
            thisInventory.SetItem(Index, null); // Clear the current item in the inventory
            Item = null; // Clear the current item
            ClearItemParent(); // Update the UI to reflect the absence of an item
        }
    }
}
