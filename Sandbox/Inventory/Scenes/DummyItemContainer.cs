using Godot;

namespace Template.Inventory;

public partial class DummyItemContainer : ItemContainer
{
    private const float InitialSmoothFactor = 0.05f;
    private const float LerpBackToOneFactor = 0.01f;

    private CursorItemContainer _cursor;
    private float _currentSmoothFactor;
    private Vector2 _offset;
    private DummyTarget _dummyTarget;
    private ItemContainer _target;

    [OnInstantiate]
    private void Init(Vector2 position, DummyTarget targetType, ItemContainer target = null)
    {
        Position = position;
        _dummyTarget = targetType;
        _target = target;
    }

    public override void _Ready()
    {
        _currentSmoothFactor = InitialSmoothFactor;
        _offset = CustomMinimumSize * 0.5f;

        if (_dummyTarget == DummyTarget.Cursor)
        {
            _cursor = Services.Get<CursorItemContainer>();
            _cursor.Hide();
        }
        else
        {
            _target.Sprite.Hide();
            _target.Count.Hide();
        }
    }

    public override void _PhysicsProcess(double delta)
    {
        Vector2 target;

        if (_dummyTarget == DummyTarget.Cursor)
        {
            target = GetGlobalMousePosition() - _offset;
        }
        else
        {
            target = _target.GlobalPosition;
        }

        float distance = Position.DistanceTo(target);

        Position = Position.MoveToward(target, distance * _currentSmoothFactor);
        _currentSmoothFactor = Mathf.Lerp(_currentSmoothFactor, 1, LerpBackToOneFactor);

        if (distance < 1)
        {
            if (_dummyTarget == DummyTarget.Cursor)
            {
                _cursor.Show();
            }
            else
            {
                _target.Sprite.Show();
                _target.Count.Show();
            }
            
            QueueFree();
        }
    }
}

public enum DummyTarget
{
    Cursor,
    Inventory
}
