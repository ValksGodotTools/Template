using Godot;

namespace Template.Inventory;

[SceneTree]
public partial class CursorItemContainer : Node2D
{
    private const float InitialSmoothFactorMouse = 0.05f;
    private const float LerpBackToOneFactor = 0.01f;
    private const float SmoothFactorTargetContainer = 0.1f;

    private Inventory _inventory;
    private ItemContainer _itemContainer;
    private ItemContainer _targetContainer;

    private Vector2 _offset;
    private float _currentSmoothFactor;
    private bool _lerpTowardsTargetContainer;

    public override void _Ready()
    {
        _inventory = new(1);
        _inventory.OnItemChanged += HandleItemChanged;

        _itemContainer = _.ItemContainer;
        _itemContainer.MouseFilter = Control.MouseFilterEnum.Ignore;

        _offset = _itemContainer.CustomMinimumSize * 0.5f;
        _currentSmoothFactor = InitialSmoothFactorMouse;

        Show();
    }

    public override void _PhysicsProcess(double delta)
    {
        if (_lerpTowardsTargetContainer)
        {
            LerpTowardsTargetContainer();
        }
        else
        {
            LerpTowardsMouse();
        }
    }

    private void LerpTowardsTargetContainer()
    {
        _itemContainer.Position = _itemContainer.Position.Lerp(_targetContainer.GlobalPosition, SmoothFactorTargetContainer);

        if (_itemContainer.Position.DistanceTo(_targetContainer.GlobalPosition) < 1)
        {
            _lerpTowardsTargetContainer = false;
            ClearItem();
            _targetContainer.ShowItem();
        }
    }

    private void LerpTowardsMouse()
    {
        Vector2 target = GetGlobalMousePosition() - _offset;
        float distance = _itemContainer.Position.DistanceTo(target);

        _itemContainer.Position = _itemContainer.Position.MoveToward(target, distance * _currentSmoothFactor);
        _currentSmoothFactor = Mathf.Lerp(_currentSmoothFactor, 1, LerpBackToOneFactor);
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

    public bool IsLerpingTowardsTargetContainer()
    {
        return _lerpTowardsTargetContainer;
    }

    public new void SetPosition(Vector2 position)
    {
        _itemContainer.Position = position;
    }

    public void StartLerpingTowardsTargetContainer(ItemContainer targetContainer)
    {
        _targetContainer = targetContainer;
        _targetContainer.HideItem();
        _lerpTowardsTargetContainer = true;
    }

    private void ResetSmoothFactor()
    {
        _currentSmoothFactor = InitialSmoothFactorMouse;
    }

    private void HandleItemChanged(int index, Item item)
    {
        _itemContainer.SetItem(item);
    }

    [OnInstantiate]
    private void Init()
    {

    }
}
