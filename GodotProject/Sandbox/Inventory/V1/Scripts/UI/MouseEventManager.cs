namespace Template.InventoryV1;

public class MouseEventManager()
{
    public bool MouseIsOnSlot { get; private set; }
    public ItemContainerMouseEventArgs ActiveSlot { get; private set; }

    public void OnMouseEntered(ItemContainerMouseEventArgs args)
    {
        MouseIsOnSlot = true;
        ActiveSlot = args;
    }

    public void OnMouseExited(ItemContainerMouseEventArgs args)
    {
        MouseIsOnSlot = false;
        ActiveSlot = args;
    }
}
