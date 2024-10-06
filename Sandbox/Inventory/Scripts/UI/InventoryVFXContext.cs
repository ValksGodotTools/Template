using Godot;

namespace Template.Inventory;

public class InventoryVFXContext(CanvasLayer ui, InventoryVFX inventoryVisualEffects, ItemContainer[] itemContainers, CursorItemContainer cursorItemContainer, Inventory inventory, Inventory cursorInventory)
{
    public CanvasLayer UI { get; } = ui;
    public InventoryVFX VFX { get; } = inventoryVisualEffects;
    public ItemContainer[] ItemContainers { get; } = itemContainers;
    public CursorItemContainer CursorItemContainer { get; } = cursorItemContainer;
    public Inventory Inventory { get; } = inventory;
    public Inventory CursorInventory { get; } = cursorInventory;
}
