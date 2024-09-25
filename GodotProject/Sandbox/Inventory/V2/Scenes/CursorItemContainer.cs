using Godot;

namespace Template.InventoryV2;

[SceneTree]
public partial class CursorItemContainer : Node2D
{
    private ItemContainer _itemContainer;
    private Inventory _inventory;
    private Vector2 _offset;

    public override void _Ready()
    {
        _inventory = new(1);
        _inventory.OnItemChanged += HandleItemChanged;

        _itemContainer = _.ItemContainer;
        _offset = _itemContainer.CustomMinimumSize * 0.5f;

        Show();
    }

    public override void _PhysicsProcess(double delta)
    {
        Vector2 target = GetGlobalMousePosition() - _offset;
        float distance = _itemContainer.Position.DistanceTo(target);

        _itemContainer.Position = _itemContainer.Position.MoveToward(target, distance * 0.1f);
    }

    public void SetItem(Item item)
    {
        _inventory.SetItem(0, item);
    }

    public new void SetPosition(Vector2 position)
    {
        _itemContainer.Position = position;
    }

    private void HandleItemChanged(int index, Item item)
    {
        _itemContainer.SetItem(item);
    }
}
