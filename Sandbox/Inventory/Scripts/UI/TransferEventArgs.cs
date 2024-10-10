namespace Template.Inventory;

public class TransferEventArgs(bool stacking, int fromIndex, ItemContainer targetItemContainer)
{
    public bool AreSameType { get; } = stacking;
    public int FromIndex { get; } = fromIndex;
    public ItemContainer TargetItemContainer { get; } = targetItemContainer;
}
