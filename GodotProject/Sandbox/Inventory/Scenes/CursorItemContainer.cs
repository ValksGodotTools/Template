using Godot;

namespace Template.Inventory;

[SceneTree]
public partial class CursorItemContainer : Node2D
{
    private const float InitialSmoothFactor = 0.05f;
    private const float LerpBackToOneFactor = 0.01f;

    private ItemContainer _itemContainer;
    private Inventory _inventory;
    private Vector2 _offset;
    private float _currentSmoothFactor;

    public override void _Ready()
    {
        _inventory = new(1);
        _inventory.OnItemChanged += HandleItemChanged;

        _itemContainer = _.ItemContainer;
        _offset = _itemContainer.CustomMinimumSize * 0.5f;
        _currentSmoothFactor = InitialSmoothFactor;

        Show();
    }

    public override void _PhysicsProcess(double delta)
    {
        Vector2 target = GetGlobalMousePosition() - _offset;
        float distance = _itemContainer.Position.DistanceTo(target);

        _itemContainer.Position = _itemContainer.Position.MoveToward(target, distance * _currentSmoothFactor);
        _currentSmoothFactor = Mathf.Lerp(_currentSmoothFactor, 1, LerpBackToOneFactor);
    }

    public void SetCurrentSpriteFrame(int frame)
    {
        _itemContainer.SetCurrentSpriteFrame(frame);
    }

    public int GetCurrentSpriteFrame()
    {
        return _itemContainer.GetCurrentSpriteFrame();
    }

    public void SetItem(Item item)
    {
        _inventory.SetItem(0, item);
        ResetSmoothFactor();
    }

    public Item GetItem()
    {
        return _inventory.GetItem(0);
    }

    public bool HasItem()
    {
        return _inventory.HasItem(0);
    }

    public void ClearItem()
    {
        _inventory.ClearItem(0);
    }

    public new void SetPosition(Vector2 position)
    {
        _itemContainer.Position = position;
    }

    private void ResetSmoothFactor()
    {
        _currentSmoothFactor = InitialSmoothFactor;
    }

    private void HandleItemChanged(int index, Item item)
    {
        _itemContainer.SetItem(item);
    }
}