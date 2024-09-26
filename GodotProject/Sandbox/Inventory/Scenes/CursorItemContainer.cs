using Godot;

namespace Template.Inventory;

[SceneTree]
public partial class CursorItemContainer : ItemContainer
{
    private const float InitialSmoothFactor = 0.05f;
    private const float LerpBackToOneFactor = 0.01f;

    private Vector2 _offset;
    private float _currentSmoothFactor;

    public override void _Ready()
    {
        IgnoreInputEvents(this);

        _offset = CustomMinimumSize * 0.5f;
        _currentSmoothFactor = InitialSmoothFactor;

        SetPhysicsProcess(false);

        Show();
    }

    public override void _PhysicsProcess(double delta)
    {
        Vector2 target = GetGlobalMousePosition() - _offset;
        float distance = Position.DistanceTo(target);

        Position = Position.MoveToward(target, distance * _currentSmoothFactor);
        _currentSmoothFactor = Mathf.Lerp(_currentSmoothFactor, 1, LerpBackToOneFactor);
    }

    public void ResetSmoothFactor()
    {
        _currentSmoothFactor = InitialSmoothFactor;
    }

    private static void IgnoreInputEvents(Control control)
    {
        control.MouseFilter = MouseFilterEnum.Ignore;
        control.SetProcessInput(false);
    }

    [OnInstantiate]
    private void Init()
    {

    }
}
